using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ui;
using UnityEngine;
using utils;
using utils.XML;

namespace AudioAnalysis
{
    public static class MultiRangeAnalysis
    {
        [SerializeField]
        private static int numberOfRanges = 4;

        private static string _clipName;
        private static float _clipFrequency;
        private static float[] _totalSamples;
        private static int _clipSamples;
        private static int _clipChannels;

        public static void Init(AudioClip clip)
        {
            _clipName = clip.name;
            _clipFrequency = clip.frequency;
            _clipSamples = clip.samples;
            _clipChannels = clip.channels;
            _totalSamples = new float[_clipSamples * _clipChannels];
            clip.GetData(_totalSamples, 0);
        }
        
        public static LevelDescription AnalyseMusic(string saveFilePath)
        {
            float[] BPMs = AudioTools.GetBPM(_totalSamples, _clipSamples, _clipChannels, _clipFrequency);
            AnalyseSlider.Progress.Update();
            
            float[][] fftData = AudioTools.FFT(_totalSamples, _clipSamples, _clipChannels, AudioTools.SampleSize);
            AnalyseSlider.Progress.Update();
            
            float[][] amplitudeEvolutionPerFrequency = new float[fftData[0].Length][];
            
            for (int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                amplitudeEvolutionPerFrequency[i] = new float[fftData.Length];
            }
            AnalyseSlider.Progress.Update();
            
            for(int i = 0; i < fftData[0].Length; i++)
            {
                Debug.Assert(fftData[i].Length == amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < fftData.Length;j++)
                {
                    amplitudeEvolutionPerFrequency[i][j] = fftData[j][i];
                }
            }
            AnalyseSlider.Progress.Update();

            bool[][] maximums = new bool[amplitudeEvolutionPerFrequency.Length][];
            for(int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                maximums[i] = AudioTools.TimewiseLocalMaximums(amplitudeEvolutionPerFrequency[i], 30);
            }
            AnalyseSlider.Progress.Update();

            //TODO Faire les moyennes sur des ranges de fréquence et renvoyer un LevelDescription (et sauvegarder cet objet en XML)

            LevelDescription level = new LevelDescription();
            level.title = _clipName + "_generated";
            List<ShapeDescription> shapes = new List<ShapeDescription>();

            float[][] noteProbability = new float[numberOfRanges][];
            int numberOfNotes = 0;
            int freqMin = 0;
            for(int i = 0; i < numberOfRanges; i++)
            {
                int freqMax = (int)(Mathf.Exp((i+1)*Mathf.Log(AudioTools.SampleSize) / numberOfRanges ));
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
            AnalyseSlider.Progress.Update();
            
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
                    shape.timeToPress = AudioTools.timeFromIndex(j, _clipFrequency) * AudioTools.SampleSize;
                    shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                    shapes.Add(shape);
                }
            }
            AnalyseSlider.Progress.Update();
            
            level.shapes = shapes.ToArray();
            XmlHelpers.SerializeToXML<LevelDescription>(saveFilePath, level);
            AnalyseSlider.Progress.Update();
            
            return level;
        }
    }
}