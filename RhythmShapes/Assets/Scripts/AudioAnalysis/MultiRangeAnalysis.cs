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
        private static int numberOfRanges = 3;

        [SerializeField]
        public static float minimalNoteDelay = 0.1f;
        [SerializeField]
        public static float analysisThreshold = 0.3f;
        public static float doubleNoteAnalysisThreshold = 0.4f;
        private static string _clipName;
        private static int _clipFrequency;
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
            float bpm = BPMAnalyzer.GetBPM(_totalSamples, _clipSamples, _clipChannels, _clipFrequency);
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
            
            LevelDescription level = new LevelDescription { title = _clipName };
            level.minimalNoteDelay = minimalNoteDelay;
            level.analysisThreshold = analysisThreshold;
            level.doubleNoteAnalysisThreshold = doubleNoteAnalysisThreshold;
            List<ShapeDescription> shapes = new List<ShapeDescription>();

            float[][] noteProbability = new float[numberOfRanges][];
            int numberOfNotes = 0;
            int numberOfDoubleNotes = 0;
            int freqMin = 0;
            for(int i = 0; i < numberOfRanges; i++)
            {
                int freqMax = (int) Mathf.Pow( 200f, (i+1)*Mathf.Log(AudioTools.SampleSize) / (Mathf.Log(200f) * numberOfRanges) );
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
            float firstNoteTime = 0;
            int doubleNoteCycle = 0;
            // int counter = 0;
            bool[] usedTarget = new []{false, false, false, false};
            bool[] allFalse = new []{false, false, false, false};
            bool belowMinDelay = false;
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
                if (maxProbability > analysisThreshold)
                {
                    if(numberOfNotes == 0 && numberOfDoubleNotes == 0)
                    {
                        firstNoteTime = AudioTools.timeFromIndex(j, _clipFrequency) * AudioTools.SampleSize;
                        firstNoteTime = Mathf.Round(firstNoteTime * 1000) / 1000;
                    }
                    float noteTime = AudioTools.timeFromIndex(j, _clipFrequency) * AudioTools.SampleSize;
                    noteTime = PostAnalysisTools.SnapTimeToBPMGrid(noteTime, bpm, firstNoteTime, 1f / 12);
                    if( j > curatedFreqMax)
                    {
                        curatedFreqMax = j;
                    }
                    if(j < curatedFreqMin)
                    {
                        curatedFreqMin = j;
                    }
                    if (maxProbability > doubleNoteAnalysisThreshold)
                    {
                        if (noteTime - oldTime < minimalNoteDelay)
                        {
                            continue;
                        }
                        else
                        {
                            numberOfDoubleNotes++;
                            numberOfNotes++;
                            ShapeDescription shape1 = new ShapeDescription();
                            shape1.target = (shape.Target)((maxProbabilityIndex+doubleNoteCycle)%4);
                            usedTarget = allFalse;
                            usedTarget[(maxProbabilityIndex+doubleNoteCycle)%4] = true;
                            shape1.type = (shape.ShapeType)((maxProbabilityIndex) % 3);
                            shape1.timeToPress = noteTime;
                            shape1.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                            shapes.Add(shape1);
                            
                            
                            numberOfNotes++;
                            ShapeDescription shape2 = new ShapeDescription();
                            shape2.target = (shape.Target)((maxProbabilityIndex+j+doubleNoteCycle)%4==(maxProbabilityIndex+doubleNoteCycle)%4 ?(maxProbabilityIndex+j+doubleNoteCycle+1)%4 : (maxProbabilityIndex+j+doubleNoteCycle)%4);
                            usedTarget[(maxProbabilityIndex+j+doubleNoteCycle)%4==(maxProbabilityIndex+doubleNoteCycle)%4 ?(maxProbabilityIndex+j+doubleNoteCycle+1)%4 : (maxProbabilityIndex+j+doubleNoteCycle)%4] = true;
                            shape2.type = (shape.ShapeType)((maxProbabilityIndex) % 3);
                            shape2.timeToPress = noteTime;
                            shape2.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                            shapes.Add(shape2);
                            oldTime = noteTime;

                            belowMinDelay = true;
                            doubleNoteCycle++;
                        }
                    }
                    else
                    {
                        if(noteTime-oldTime < minimalNoteDelay) {
                            ShapeDescription shape = new ShapeDescription();
                            if (usedTarget[(maxProbabilityIndex+j)%4]) // if both shapes go to same target 
                            {
                                int selectedIndex = -1;
                                int k = 1;
                                while(selectedIndex == -1 && k < 4)
                                {
                                    if (!usedTarget[(maxProbabilityIndex + k) % 4]) // 4 = numberOftargets, variable à généraliser si update donnant lieu à des niveaux avec un nombre différent de targets
                                    {
                                        selectedIndex = (maxProbabilityIndex+k)% 4;
                                    }
                                    k++;
                                }
                                if(selectedIndex != -1)
                                {
                                    shape.target = (shape.Target)(selectedIndex%4);
                                    usedTarget[selectedIndex%4] = true;
                                    shape.type = (shape.ShapeType)((maxProbabilityIndex) % 3);
                                    shape.timeToPress = oldTime;
                                    shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                                    shapes.Add(shape);
                                    numberOfNotes++;
                                    if (!belowMinDelay)
                                    {
                                        belowMinDelay = true;
                                        numberOfDoubleNotes++;
                                    }
                                }
                                else
                                {
                                    // ignore other notes
                                    //Debug.Log("All Target Have Been Used" + j);
                                    // Debug.Log(usedTarget[0]+", "+usedTarget[1]+", "+usedTarget[2]+", "+usedTarget[3]);
                                }
                                
                            }
                            else
                            {
                                shape.target = (shape.Target)((maxProbabilityIndex+j)%4);
                                usedTarget[(maxProbabilityIndex+j)%4] = true;
                                shape.type = (shape.ShapeType)((maxProbabilityIndex) % 3);
                                shape.timeToPress = oldTime;
                                shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                                shapes.Add(shape);
                                numberOfNotes++;
                            }
                            // counter++;
                            continue;
                        }
                        else
                        {
                            numberOfNotes++;
                            ShapeDescription shape = new ShapeDescription();
                            shape.target = (shape.Target)((maxProbabilityIndex+j)%4);
                            usedTarget = allFalse;
                            usedTarget[(maxProbabilityIndex+j)%4] = true;
                            shape.type = (shape.ShapeType)((maxProbabilityIndex) % 3);
                            shape.timeToPress = noteTime;
                            shape.goRight = ((maxProbabilityIndex + j) % 2).Equals(0);
                            shapes.Add(shape);
                            oldTime = noteTime;
                            belowMinDelay = false;
                            // counter = 1;
                        }
                    }
                }
            }
            ProgressUtil.Update();

            level.numberOfNotes = numberOfNotes;
            level.numberOfDoubleNotes = numberOfDoubleNotes;
            ShapeDescription[] finalShapes = shapes.ToArray();
            while(bpm > 240)
            {
                bpm /= 2;
            }
            while (bpm < 80)
            {
                bpm *= 2;
            }
            Debug.Log("BPM = " + bpm);
            //LevelPreparator.TravelTime = 2* 60 / bpm;
            if (finalShapes != null && finalShapes.Length > 0)
            {
                PostAnalysisTools.RoundTimingToMilliseconds(finalShapes);
                ProgressUtil.Update();
                int fbi = PostAnalysisTools.FirstBeatIndex(finalShapes, bpm);
                PostAnalysisTools.snapNotesToBPMGrid(finalShapes, bpm, finalShapes[fbi].timeToPress, 1f / 12);
                ProgressUtil.Update();
                PostAnalysisTools.RythmicPatternRepetition(finalShapes, bpm, finalShapes[fbi].timeToPress, 0.91f);
            }
            level.shapes = shapes.ToArray();
            ProgressUtil.Update();
            
            return level;
        }
    }
}