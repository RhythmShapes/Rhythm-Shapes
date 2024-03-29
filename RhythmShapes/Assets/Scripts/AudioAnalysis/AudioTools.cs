using System;
using UnityEngine;
using DSPLib;
using System.Numerics;
using System.Collections.Generic;

namespace AudioAnalysis
{
    public static class AudioTools
    {
        public const int SampleSize = 2048;

        public static float[] preprocessAudioData(float[] totalSamples, int clipSamples, int channels)
        //Processes the clip from the given audio clip and return the samples combined as mono in a float[]  
        {
            try
            {
                float[] samples = new float[clipSamples];
                float averageValue = 0;
                for (int i = 0; i < totalSamples.Length; i++)
                {
                    averageValue += totalSamples[i];
                    if ((i + 1) % channels == 0)
                    {
                        samples[(int)i / channels] = averageValue / channels;
                        averageValue = 0;
                    }
                }


                return samples;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return new float[0];
            }
        }

        public static float[][] FFT(float[] totalSamples, int clipSamples, int channels, int sampleSize)
        {
            float[] samples = preprocessAudioData(totalSamples, clipSamples,  channels);
            int iterations = samples.Length / sampleSize;
            FFT fft = new FFT();
            fft.Initialize((UInt32)sampleSize);
            double[] sampleChunk = new double[sampleSize];
            double[][] scaledFFTSpectrum = new double[iterations][];
            float[][] spectrum = new float[iterations][];
            for (int i = 0; i < iterations; i++)
            {
                Array.Copy(samples, i * sampleSize, sampleChunk, 0, sampleSize);
                double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hamming, (uint)sampleSize);
                double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
                double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

                Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
                scaledFFTSpectrum[i] = DSPLib.DSP.ConvertComplex.ToMagnitude(fftSpectrum);
                scaledFFTSpectrum[i] = DSP.Math.Multiply(scaledFFTSpectrum[i], scaleFactor);
            }

            for (int i = 0; i < scaledFFTSpectrum.Length; i++)
            {
                spectrum[i] = Array.ConvertAll(scaledFFTSpectrum[i], x => (float)x); //convert doubles to floats
            }
            return spectrum;
        }

        public static float timeFromIndex(int index, AudioClip clip)
        //Gives the time in seconds corresponding to the given preprocessed samples index.
        {
            return timeFromIndex(index, clip.frequency);
        }

        public static float timeFromIndex(int index, float clipSampleRate)
            //Gives the time in seconds corresponding to the given preprocessed samples index.
        {
            return index / clipSampleRate;
        }

        public static int indexFromTime(float time, AudioClip clip)
        //Gives the index in the preprocessed samples corresponding to the given time in seconds.
        {
            return indexFromTime(time, clip.samples, clip.length);
        }

        public static int indexFromTime(float time, float clipSample, float clipLength)
            //Gives the index in the preprocessed samples corresponding to the given time in seconds.
        {
            return Mathf.FloorToInt(time * clipSample / clipLength);
        }


        public static bool[] TimewiseLocalMaximums(float[] data, int timeWindow)
        /*Implement a function that returns
         * a bool[] where bool[i] is true if and only if 
         * fftData[i] is a LocalMaximum within the timeWindow time range 
         * The "data" argument is considered to be an evolution of the same data through time*/        
        {

            bool[] isMax = new bool[data.Length];
            for(int i = 0; i < data.Length; i++)
            {
                isMax[i] = true;
                int start = Mathf.Max(0, i - timeWindow/2);
                int end = Mathf.Min(data.Length, i + timeWindow/2);
                for(int j = start; j < end; j++)
                {
                    if (data[i] == 0 ||data[j] > data[i] && j!=i)
                    {
                        isMax[i] = false;
                    }
                }
            }
            return isMax;
        }

        public static float AverageEnergy(float[] data)
        {
            float sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i]*data[i];
            }
            return Mathf.Sqrt(sum) / data.Length;
        }

        public static float[] GetBPMs(float[] totalSamples, int clipSamples, int channels, float frequency)
        {
            float[][] spectrum = FFT(totalSamples, clipSamples,  channels, SampleSize);
            float[] BPMS = new float[spectrum.Length];
            float[] averageEnergies = new float[spectrum.Length];
            for(int i = 0; i < spectrum.Length; i++)
            {
                averageEnergies[i] = AverageEnergy(spectrum[i]);
            }
            float averageEnergy = 0;
            for(int i = 0; i<averageEnergies.Length; i++)
            {
                averageEnergy += averageEnergies[i];
            }
            averageEnergy/= averageEnergies.Length;

            int lastBeatIndex = 0;
            float currentBPM=0;
            for(int i = 0; i < averageEnergies.Length; i++)
            {
                if(averageEnergies[i] > averageEnergy && i > lastBeatIndex)
                {
                    currentBPM = 60 / ((float)(i - lastBeatIndex)/frequency);
                    lastBeatIndex = i;
                }
                BPMS[i] = currentBPM;
            }
            return BPMS;
        }

        public static float GetBPM(float[] totalSamples, int clipSamples, int channels, int frequency)
        {
            float[][] spectrum = FFT(totalSamples, clipSamples, channels, SampleSize);
            float[] BPMS = new float[spectrum.Length];
            float[] averageEnergies = new float[spectrum.Length];
            for (int i = 0; i < spectrum.Length; i++)
            {
                averageEnergies[i] = AverageEnergy(spectrum[i]);
            }
            float averageEnergy = 0;
            for (int i = 0; i < averageEnergies.Length; i++)
            {
                averageEnergy += averageEnergies[i];
            }
            averageEnergy /= averageEnergies.Length;


            int lastBeatIndex = 0;
            float currentBPM = 0;
            float BPM = 0;
            for (int i = 0; i < averageEnergies.Length; i++)
            {
                if (averageEnergies[i] > averageEnergy && i > lastBeatIndex)
                {
                    currentBPM = 60/((float)(i - lastBeatIndex) / frequency);
                    lastBeatIndex = i;
                }
                BPMS[i] = currentBPM;
                BPM += currentBPM;
            }
            return BPM/BPMS.Length;
        }

        public static float[] Normalize(float[] data)
        {
            float ratio = 1/Mathf.Max(data);
            for(int i = 0; i < data.Length; i++)
            {
                data[i] *= ratio;
            }
            return data;
        }

        public static float[] GetSectionsTimestamps(float[] signal, int kernelSize, float frequency)
        {
            signal = Normalize(signal);
            List<float> result = new List<float>();
            float[] kernel = new float[kernelSize];
            for (int i = 0; i < kernelSize; i++)
            {
                kernel[i] = (float)1 / kernelSize;
            }
            float[] convolution = Convolution.Convolve1D(signal, kernel);

            float[] relevantData = new float[signal.Length];

            for (int i = 0; i < signal.Length; i++)
            {
                relevantData[i] = convolution[kernelSize / 2 + i - 1];
            }

            for (int i = 0; i < kernelSize; i++)
            {
                if(i == kernelSize / 2)
                {
                    kernel[i] = 0;
                }
                else
                {
                    kernel[i] = 1/(float)(i-kernelSize/2);
                }
            }

            float[] contrastKernel = { -1, -1, 1, 1, 1 };

            convolution = Convolution.Convolve1D(convolution, contrastKernel);
            
            for (int i = 0; i < signal.Length; i++)
            {
                relevantData[i] = convolution[contrastKernel.Length / 2 + i-1];
            }

            List<float> timestamps = new List<float>();
            for(int i = 0; i<relevantData.Length; i++)
            {
                if(Mathf.Abs(relevantData[i]) > 1.85)
                {
                    if (timestamps.Count <= 0 || timeFromIndex(i, frequency) - timestamps[timestamps.Count - 1] > 0.1)
                    {
                        timestamps.Add(timeFromIndex(i, frequency));
                    }
                }
            }
            
            return timestamps.ToArray();
        }


    }
}