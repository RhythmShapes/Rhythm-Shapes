using System.Collections;
using System.Collections.Generic;
using System.Threading;
using shape;
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

            float[] averageAmplitudePerFrequency = new float[numberOfRanges];
            for(int i = 0; i < fftData.Length; i++)
            {
                Debug.Assert(fftData[i].Length == amplitudeEvolutionPerFrequency.Length);
                for(int j = 0; j < fftData[i].Length;j++)
                {
                    amplitudeEvolutionPerFrequency[j][i] = fftData[i][j];
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

            float oldTime = 0;
            // int counter = 0;
            bool[] usedTarget = new []{false, false, false, false};
            bool[] allFalse = new []{false, false, false, false};
            int curatedFreqMin = 20000;
            int curatedFreqMax = 20;
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
                    float noteTime = AudioTools.timeFromIndex(j, _clipFrequency) * AudioTools.SampleSize;
                    if( j > curatedFreqMax)
                    {
                        curatedFreqMax = j;
                    }
                    if(j < curatedFreqMin)
                    {
                        curatedFreqMin = j;
                    }
                    if(noteTime-oldTime < minimalNoteDelay) {
                        numberOfNotes++;
                        ShapeDescription shape = new ShapeDescription();
                        if (usedTarget[maxProbabilityIndex]) // if both shapes go to same target 
                        {
                            int selectedIndex = -1;
                            int k = 1;
                            while(selectedIndex == -1 && k < numberOfRanges)
                            {
                                if (!usedTarget[(maxProbabilityIndex + k) % numberOfRanges])
                                {
                                    selectedIndex = (maxProbabilityIndex+k) % numberOfRanges;
                                }
                            }
                            if(selectedIndex != -1)
                            {
                                shape.target = (shape.Target)selectedIndex;
                                usedTarget[selectedIndex] = true;
                                shape.type = (shape.ShapeType)((selectedIndex + j) % 3);
                                shape.timeToPress = oldTime;
                                shape.goRight = ((selectedIndex + j) % 2).Equals(0);
                                shapes.Add(shape);
                                numberOfNotes++;
                                }

                            /*if (!usedTarget[(maxProbabilityIndex + 1) % 4])
                            {
                                shape.target = (shape.Target)((maxProbabilityIndex + 1) % 4);
                                usedTarget[(maxProbabilityIndex + 1) % 4] = true;
                                shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                                shape.timeToPress = oldTime;
                                shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                                shapes.Add(shape);
                            }
                            else if (!usedTarget[(maxProbabilityIndex + 2) % 4])
                            {
                                shape.target = (shape.Target)((maxProbabilityIndex + 2) % 4);
                                usedTarget[(maxProbabilityIndex + 2) % 4] = true;
                                shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                                shape.timeToPress = oldTime;
                                shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                                shapes.Add(shape);
                            }
                            else if (!usedTarget[(maxProbabilityIndex + 3) % 4])
                            {
                                shape.target = (shape.Target)((maxProbabilityIndex + 3) % 4);
                                usedTarget[(maxProbabilityIndex + 3) % 4] = true;
                                shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                                shape.timeToPress = oldTime;
                                shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                                shapes.Add(shape);
                            }*/
                            else
                            {
                                Debug.LogError("All Target Have Been Used" + j);
                                // Debug.Log(usedTarget[0]+", "+usedTarget[1]+", "+usedTarget[2]+", "+usedTarget[3]);
                            }
                            
                        }
                        else
                        {
                            shape.target = (shape.Target)maxProbabilityIndex;
                            usedTarget[maxProbabilityIndex] = true;
                            shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                            shape.timeToPress = oldTime;
                            shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                            shapes.Add(shape);
                            numberOfNotes++;
                            }
                        // counter++;
                    }
                    else
                    {
                        numberOfNotes++;
                        ShapeDescription shape = new ShapeDescription();
                        shape.target = (shape.Target)maxProbabilityIndex;
                        usedTarget = allFalse;
                        usedTarget[maxProbabilityIndex] = true;
                        shape.type = (shape.ShapeType)((maxProbabilityIndex + j) % 3);
                        shape.timeToPress = noteTime;
                        shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                        shapes.Add(shape);
                        oldTime = noteTime;
                        // counter = 1;
                    }
                }
            }
            for(int i = 0;i< shapes.Count;i++)
            {
                /*TODO remap the shape type according to the detected frequency of the 
                 * note relatively to the maxFreq and minFreq of the audioClip*/
            }
            AnalyseSlider.Progress.Update();
            
            level.shapes = shapes.ToArray();
            XmlHelpers.SerializeToXML<LevelDescription>(saveFilePath, level);
            AnalyseSlider.Progress.Update();
            
            return level;
        }
    }
}