using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{
    public static class MultiRangeAnalysis
    {
        [SerializeField]
        private static int numberOfRanges = 4;
        public static LevelDescription AnalyseMusic(AudioClip clip, string levelName)
        {
            const int SAMPLE_SIZE = 2048;
            float[] BPMs = AudioTools.GetBPM(clip);
            float[][] fftData = AudioTools.FFT(clip,SAMPLE_SIZE);
            float[][] amplitudeEvolutionPerFrequency = new float[fftData[0].Length][];
            for (int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                amplitudeEvolutionPerFrequency[i] = new float[fftData.Length];
            }
            for(int i = 0; i < fftData[0].Length; i++)
            {
                Debug.Assert(fftData[i].Length == amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < fftData.Length;j++)
                {
                    amplitudeEvolutionPerFrequency[i][j] = fftData[j][i];
                }
            }

            bool[][] maximums = new bool[amplitudeEvolutionPerFrequency.Length][];
            for(int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                maximums[i] = AudioTools.TimewiseLocalMaximums(amplitudeEvolutionPerFrequency[i], 30);
            }

            //TODO Faire les moyennes sur des ranges de fréquence et renvoyer un LevelDescription (et sauvegarder cet objet en XML)

            LevelDescription level = new LevelDescription();
            level.title = clip.name + "_generated";
            List<ShapeDescription> shapes = new List<ShapeDescription>();

            float[][] noteProbability = new float[numberOfRanges][];
            int numberOfNotes = 0;
            int freqMin = 0;
            for(int i = 0; i < numberOfRanges; i++)
            {
                int freqMax = (int)(Mathf.Exp((i+1)*Mathf.Log(SAMPLE_SIZE) / numberOfRanges ));
                if(freqMax >= maximums.Length) { freqMax = maximums.Length-1; }
                noteProbability[i] = new float[amplitudeEvolutionPerFrequency[0].Length];
                for(int j = 0; j < noteProbability[i].Length; j++)
                {
                    noteProbability[i][j] = 0;
                    for(int l = freqMin; l < freqMax; l++)
                    {
                        if (maximums[l][j])
                        {
                            noteProbability[i][j]++;
                        }
                    }
                    noteProbability[i][j] /= (freqMax - freqMin);
                    
                }
                freqMin= freqMax;
            }
            for (int j = 0; j < noteProbability[0].Length; j++)
            {
                float maxProbability = 0;
                int maxProbabilityIndex = 0;
                for (int i = 0; i < numberOfRanges; i++)
                {
                    if(maxProbability< noteProbability[i][j])
                    {
                        maxProbability = noteProbability[i][j];
                        maxProbabilityIndex = i;
                    }
                }
                if (maxProbability > 0.3)
                {
                    numberOfNotes++;
                    ShapeDescription shape = new ShapeDescription();
                    shape.target = (shape.Target)maxProbabilityIndex;
                    shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                    shape.timeToPress = AudioTools.timeFromIndex(j, clip) * SAMPLE_SIZE;
                    shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                    shapes.Add(shape);
                }
            }
            level.shapes = shapes.ToArray();
            XmlHelpers.SerializeToXML<LevelDescription>("Assets/Resources/Levels/" + levelName + "/data.xml", level);
            return level;
        }
    }
}