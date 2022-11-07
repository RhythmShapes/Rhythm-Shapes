using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{
    public class MultiRangeAnalysis
    {
        [SerializeField]
        private int numberOfRanges = 4;
        public LevelDescription AnalyseMusic(AudioClip clip)
        {
            const int SAMPLE_SIZE = 2048;
            float[] BPMs = AudioTools.GetBPM(clip);
            float[][] fftData = AudioTools.FFT(clip,SAMPLE_SIZE);
            float[][] amplitudeEvolutionPerFrequency = new float[fftData[0].Length][];
            for (int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                amplitudeEvolutionPerFrequency[i] = new float[fftData.Length];
            }
            for(int i = 0; i < fftData.Length; i++)
            {
                Debug.Assert(fftData[i].Length == amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < fftData.Length;)
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
            float[][] noteOccurrence = new float[numberOfRanges][];
            int numberOfNotes = 0;
            int freqMin = 0;
            for(int i = 0; i < numberOfRanges; i++)
            {
                int freqMax = (int)(Mathf.Log(i)*numberOfRanges/Mathf.Log(SAMPLE_SIZE));
                noteOccurrence[i] = new float[amplitudeEvolutionPerFrequency[0].Length];
                for(int j = 0; j < noteOccurrence[i].Length; j++)
                {
                    noteOccurrence[i][j] = 0;
                    for(int k = freqMin; k < freqMax; k++)
                    {
                        if (maximums[k][j])
                        {
                            noteOccurrence[i][j]++;
                        }
                    }
                    noteOccurrence[i][j] /= (freqMax - freqMin);
                    if(noteOccurrence[i][j] > 0.5)
                    {
                        numberOfNotes++;
                    }
                }
                freqMin= freqMax;
            }

            LevelDescription level = new LevelDescription();
            level.title = clip.name + "_generated";
            level.shapes = new ShapeDescription[numberOfNotes];
            int k = 0;
            for(int i = 0; i < numberOfRanges; i++)
            {
                for(int j = 0; j < noteOccurrence[i].Length; j++)
                {
                    if(noteOccurrence[i][j] > 0.5)
                    {
                        ShapeDescription shape = new ShapeDescription();
                        shape.target = (shape.Target)i;
                        shape.type = (shape.ShapeType)((i + j) % 3);
                        shape.timeToPress = AudioTools.timeFromIndex(j,clip);
                        shape.goRight = ((i + j) % 2).Equals(0);
                        level.shapes[k] = shape;
                        k++;
                    }
                }

            }
            
            return level;
        }
    }
}