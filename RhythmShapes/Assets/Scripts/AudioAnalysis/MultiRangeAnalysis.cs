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

        [SerializeField]
        private static float minimalNoteDelay = 0.1f;
        [SerializeField]
        private static float analysisThreshold = 0.3f;
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
        
        public static LevelDescription AnalyseMusic()
        {
            float[] BPMs = AudioTools.GetBPM(_totalSamples, _clipSamples, _clipChannels, _clipFrequency);
            ProgressUtil.Update();
            
            float[][] fftData = AudioTools.FFT(_totalSamples, _clipSamples, _clipChannels, AudioTools.SampleSize);
            ProgressUtil.Update();
            
            float[][] amplitudeEvolutionPerFrequency = new float[fftData[0].Length][];
            
            for (int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                amplitudeEvolutionPerFrequency[i] = new float[fftData.Length];
            }
            ProgressUtil.Update();

            float[] averageAmplitudePerFrequency = new float[numberOfRanges];
            for(int i = 0; i < fftData.Length; i++)
            {
                Debug.Assert(fftData[i].Length == amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < fftData[i].Length;j++)
                {
                    amplitudeEvolutionPerFrequency[j][i] = fftData[i][j];
                }
            }
            ProgressUtil.Update();

            bool[][] maximums = new bool[amplitudeEvolutionPerFrequency.Length][];
            for(int i = 0; i < amplitudeEvolutionPerFrequency.Length; i++)
            {
                maximums[i] = AudioTools.TimewiseLocalMaximums(amplitudeEvolutionPerFrequency[i], 30);
            }
            ProgressUtil.Update();

            //TODO Faire les moyennes sur des ranges de fréquence et renvoyer un LevelDescription (et sauvegarder cet objet en XML)

            LevelDescription level = new LevelDescription { title = _clipName };
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
            ProgressUtil.Update();

            float oldTime = 0;
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
                if (maxProbability > analysisThreshold)
                {
                    float noteTime= AudioTools.timeFromIndex(j, _clipFrequency) * AudioTools.SampleSize;
                    if(noteTime-oldTime < minimalNoteDelay) { continue; }
                    numberOfNotes++;
                    ShapeDescription shape = new ShapeDescription();
                    shape.target = (shape.Target)maxProbabilityIndex;
                    shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                    shape.timeToPress = noteTime;
                    shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                    shapes.Add(shape);
                    oldTime = noteTime;
                }
            }
            
            level.shapes = shapes.ToArray();
            ProgressUtil.Update();
            
            return level;
        }
    }
}