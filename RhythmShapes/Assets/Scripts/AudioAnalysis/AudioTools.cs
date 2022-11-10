using System;
using UnityEngine;
using DSPLib;
using System.Numerics;

namespace AudioAnalysis
{
    public static class AudioTools
    {

        public static float[] preprocessAudioData(AudioClip clip)
        //Processes the clip from the given audio clip and return the samples combined as mono in a float[]  
        {
            try
            {
                float[] totalSamples = new float[clip.samples * clip.channels];
                float[] samples = new float[clip.samples];
                float averageValue = 0;
                clip.GetData(totalSamples, 0);
                for (int i = 0; i < totalSamples.Length; i++)
                {
                    averageValue += totalSamples[i];
                    if ((i + 1) % clip.channels == 0)
                    {
                        samples[(int)i / clip.channels] = averageValue / clip.channels;
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

        public static float[][] FFT(AudioClip clip, int SAMPLE_SIZE)
        {
            float[] samples = preprocessAudioData(clip);
            int iterations = samples.Length / SAMPLE_SIZE;
            FFT fft = new FFT();
            fft.Initialize((UInt32)SAMPLE_SIZE);
            double[] sampleChunk = new double[SAMPLE_SIZE];
            double[][] scaledFFTSpectrum = new double[iterations][];
            float[][] spectrum = new float[iterations][];
            for (int i = 0; i < iterations; i++)
            {
                Array.Copy(samples, i * SAMPLE_SIZE, sampleChunk, 0, SAMPLE_SIZE);
                double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hamming, (uint)SAMPLE_SIZE);
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
            float clipSampleRate = clip.frequency;
            return index / clipSampleRate;
        }

        public static int indexFromTime(float time, AudioClip clip)
        //Gives the index in the preprocessed samples corresponding to the given time in seconds.
        {
            return Mathf.FloorToInt(time * clip.samples / clip.length);
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
                    if (data[j] > data[i] && j!=i)
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

        public static float[] GetBPM(AudioClip clip)
        {
            float[][] spectrum = FFT(clip, 2048);
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
                    currentBPM = 60 / ((float)(i - lastBeatIndex)/clip.frequency);
                    lastBeatIndex = i;
                }
                BPMS[i] = currentBPM;
            }
            return BPMS;
        }
    }
}
