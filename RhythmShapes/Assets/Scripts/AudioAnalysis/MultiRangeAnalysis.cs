using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{
    public static class MultiRangeAnalysis
    {
        [SerializeField]
        private static int numberOfRanges = 4;
        
        const int SAMPLE_SIZE = 2048;

        private static AudioClip _clip;
        private static float[] _BPMs;
        private static float[][] _fftData;
        private static float[][] _amplitudeEvolutionPerFrequency;
        private static string _clipName;
        private static float _clipFrequency;

        private static float _progress = 0f;
        private static float _progressTotal = 1f;

        public static void Init(AudioClip clip)
        {
            _clip = clip;
            _BPMs = AudioTools.GetBPM(clip);
            _fftData = AudioTools.FFT(clip, SAMPLE_SIZE);
            _amplitudeEvolutionPerFrequency = new float[_fftData[0].Length][];
            _clipName = clip.name;
            _clipFrequency = _clip.frequency;

            _progress = 0f;
            _progressTotal = _amplitudeEvolutionPerFrequency.Length * 3 + _fftData[0].Length + numberOfRanges;
        }
        
        public static LevelDescription AnalyseMusic(string saveFilePath)
        {
            Thread.Sleep(2000);
            //float[] BPMs = AudioTools.GetBPM(clip);
            //float[][] fftData = AudioTools.FFT(clip,SAMPLE_SIZE);
            //float[][] amplitudeEvolutionPerFrequency = new float[fftData[0].Length][];
            
            for (int i = 0; i < _amplitudeEvolutionPerFrequency.Length; i++, _progress++)
            {
                _amplitudeEvolutionPerFrequency[i] = new float[_fftData.Length];
            }
            for(int i = 0; i < _fftData[0].Length; i++, _progress++)
            {
                Debug.Assert(_fftData[i].Length == _amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < _fftData.Length;j++)
                {
                    _amplitudeEvolutionPerFrequency[i][j] = _fftData[j][i];
                }
            }

            bool[][] maximums = new bool[_amplitudeEvolutionPerFrequency.Length][];
            for(int i = 0; i < _amplitudeEvolutionPerFrequency.Length; i++, _progress++)
            {
                maximums[i] = AudioTools.TimewiseLocalMaximums(_amplitudeEvolutionPerFrequency[i], 30);
            }

            //TODO Faire les moyennes sur des ranges de fréquence et renvoyer un LevelDescription (et sauvegarder cet objet en XML)

            LevelDescription level = new LevelDescription();
            level.title = _clipName + "_generated";
            List<ShapeDescription> shapes = new List<ShapeDescription>();

            float[][] noteProbability = new float[numberOfRanges][];
            int numberOfNotes = 0;
            int freqMin = 0;
            for(int i = 0; i < numberOfRanges; i++, _progress++)
            {
                int freqMax = (int)(Mathf.Exp((i+1)*Mathf.Log(SAMPLE_SIZE) / numberOfRanges ));
                if(freqMax >= maximums.Length) { freqMax = maximums.Length-1; }
                noteProbability[i] = new float[_amplitudeEvolutionPerFrequency[0].Length];
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
            for (int j = 0; j < noteProbability[0].Length; j++, _progress++)
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
                    shape.timeToPress = AudioTools.timeFromIndex(j, _clipFrequency) * SAMPLE_SIZE;
                    shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                    shapes.Add(shape);
                }
            }
            level.shapes = shapes.ToArray();
            XmlHelpers.SerializeToXML<LevelDescription>(saveFilePath, level);
            return level;
        }

        public static float GetProgress()
        {
            return _progress / _progressTotal;
        }
    }
}