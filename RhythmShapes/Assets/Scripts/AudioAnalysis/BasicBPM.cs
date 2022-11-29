using System.Collections.Generic;
using ui;
using UnityEngine;
using utils;
using utils.XML;

namespace AudioAnalysis
{
    public class BasicBPM
    {
        [SerializeField]
        private static int beatWindowSize;

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
            float[][] fullSpectrum = AudioTools.FFT(_totalSamples, _clipSamples, _clipChannels, AudioTools.SampleSize);
            AnalyseSlider.Progress.Update();
            
            float[] averageEnergyPerSample = new float[fullSpectrum.Length];
            int[] beatType = new int[fullSpectrum.Length];
            float overallAverageEnergy = 0;

            for (int i = 0; i < averageEnergyPerSample.Length; i++)
            {
                averageEnergyPerSample[i] = AudioTools.AverageEnergy(fullSpectrum[i]);
                overallAverageEnergy += averageEnergyPerSample[i];
            }
            overallAverageEnergy /= averageEnergyPerSample.Length;
            AnalyseSlider.Progress.Update();

            beatType = new int[averageEnergyPerSample.Length];

            for (int i = 0; i < averageEnergyPerSample.Length; i++)
            {
                beatType[i] = 0;
                if (averageEnergyPerSample[i] < overallAverageEnergy)
                {
                    beatType[i] = -1;
                    continue;
                }

                int windowStart = Mathf.Max(0, i - beatWindowSize / 2);
                int windowEnd = Mathf.Min(averageEnergyPerSample.Length, i + beatWindowSize / 2);

                for (int j = windowStart; j < windowEnd; j++)
                {
                    if (averageEnergyPerSample[i] < averageEnergyPerSample[j])
                    {
                        beatType[i] = -1;
                        break;
                    }
                }
            }
            AnalyseSlider.Progress.Update();
            
            LevelDescription level = new LevelDescription();
            level.title = _clipName + "_generated";
            List<ShapeDescription> shapes = new List<ShapeDescription>();
            System.Random rand = new System.Random();
            for (int i = 0; i < beatType.Length; i++)
            {
                if (beatType[i] >= 0)
                {
                    ShapeDescription shape = new ShapeDescription();
                    shape.target = (shape.Target) rand.Next(4);
                    shape.type = (shape.ShapeType) rand.Next(3);
                    shape.timeToPress = AudioTools.timeFromIndex(i, _clipFrequency) * AudioTools.SampleSize;
                    shape.goRight =  rand.Next(2).Equals(0);
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