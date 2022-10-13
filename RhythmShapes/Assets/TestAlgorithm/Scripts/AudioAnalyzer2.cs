using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Numerics;
using System.Text;
using System.Threading;
using DSPLib;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(AudioSource))]
public class AudioAnalyzer2 : MonoBehaviour
{
    public bool isReady { get; private set; }
    
    // Spectrum chunk size
    [SerializeField] [Range(64, 8192)] private int spectrumSampleSize = 1024;
    
    // Sensitivity multiplier to scale the average threshold.
    // In this case, if a rectified spectral flux sample is > 1.5 times the average, it is a peak
    [SerializeField] [Min(0)] private float thresholdMultiplier = 1.5f;
    
    // Number of samples (window) to average in flux threshold calculations
    [SerializeField] [Min(0)] private int thresholdWindowSize = 50;

    private AudioClip audioClip;
    private float[] initialSamples;
    private float[] samples;
    private float sampleRate;
    private int channelsCount;
    private float[][] spectrum;
    private readonly Dictionary<float, bool> peaks = new ();
    private int currentSpectrumAnalyzed = 0;
    
    private void OnValidate() {
        spectrumSampleSize = Mathf.ClosestPowerOfTwo(spectrumSampleSize);
    }

    private void Awake()
    {
        audioClip = GetComponent<AudioSource>().clip;
        initialSamples = new float[audioClip.samples * audioClip.channels];
        samples = new float[audioClip.samples];
        sampleRate = audioClip.frequency;
        channelsCount = audioClip.channels;
        
        int iterations = samples.Length / spectrumSampleSize;
        spectrum = new float[iterations][];
        for (int i = 0; i < iterations; i++)
            spectrum[i] = new float[spectrumSampleSize];
    }

    private void Start()
    {
        // Get samples
        audioClip.GetData(initialSamples, 0);
        Debug.Log("Samples count: " + audioClip.samples + " , channels count: " + channelsCount + ", current samples size: " + initialSamples.Length);
        
        Debug.Log ("Starting spectrum analyze");
        Thread analyzeSpectrumThread = new Thread(AnalyzeSpectrum);
        analyzeSpectrumThread.Start();
        analyzeSpectrumThread.Join();
        isReady = true;
    }

    private void AnalyzeSpectrum()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        // Convert stereo to mono
        if (channelsCount > 1)
        {
           for (int i = 0, j = 0; j < initialSamples.Length; i++, j += channelsCount)
               samples[i] = (initialSamples[j] + initialSamples[j + 1]) / channelsCount;

           Debug.Log("Conversion from stereo to mono done");
        }
        else
           samples.CopyTo(initialSamples, 0);

        Debug.Log("Final samples size: " + samples.Length);

        FFT fft = new FFT ();
        fft.Initialize ((UInt32) spectrumSampleSize);

        StringBuilder sb = new StringBuilder("[\n");

        double[] sampleChunk = new double[spectrumSampleSize];
        for (int i = 0; i < spectrum.Length; i++)
        {
            Array.Copy(samples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

            // Apply our chosen FFT Window
            double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
            double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
            double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);
            
            // Perform the FFT and convert output (complex numbers) to Magnitude
            Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
            double[] scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude(fftSpectrum);
            scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);
            
            spectrum[i] = Array.ConvertAll(scaledFFTSpectrum, x => (float)x);
            
            sb.Append("{\"time\": " + GetTimeFromIndex(i).ToString().Replace(",", ".") + ", ");
            sb.Append("\"spectrum\": [");
            bool comma = false;
            foreach (var v in scaledFFTSpectrum)
            {
                if(comma)
                    sb.Append(",");
                
                sb.Append(Math.Round(v, 5).ToString().Replace(",", "."));
                comma = true;
            }
            
            sb.Append("]}");
            
            if(i < spectrum.Length - 1)
                sb.Append(",");
            sb.Append("\n");
        }
        
        sb.Append("]");
        File.WriteAllText("Assets/TestAlgorithm/json/test.json", sb.ToString());
        
        Debug.Log("Frequency conversion done");

        for (int i = 0; i < spectrum.Length; i++)
        {
            currentSpectrumAnalyzed = i;
            
            Debug.Log("======== Analyzing spectrum frame n°" + i + " / " + (spectrum.Length - 1) + " (" + (GetAnalyzeProgression() * 100) + "%) ========");
            AnalyzeSpectrumFrame(i);
            Debug.Log("==================================================================================================");
        }
        
        stopwatch.Stop();
        Debug.Log("Onset analyze done in : " + (stopwatch.ElapsedMilliseconds / 1000) + " seconds");
        
        IReadOnlyDictionary<float, bool> peaksList = GetPeaks();
        foreach (var pair in peaksList)
            Debug.Log("At time : " + pair.Key + "  : " + pair.Value);
    }
    
    private void AnalyzeSpectrumFrame(int frame)
    {
        float[] currentSpectrum = spectrum[frame];

        //Calculate rectified spectral flux
        float[] rectifiedSpectralFlux = new float[currentSpectrum.Length];
        Array.Clear(rectifiedSpectralFlux, 0, rectifiedSpectralFlux.Length);
        
        // Aggregate positive changes in spectrum data
        for (int i = 1; i < currentSpectrum.Length; i++)
        {
            rectifiedSpectralFlux[i] += Mathf.Max(0f, currentSpectrum[i] - currentSpectrum[i - 1]);
            
            //Debug.Log("Rectified spectral flux: " + rectifiedSpectralFlux[i]);
        }

        // How many samples in the past and future we include in our average
        int windowSize = thresholdWindowSize / 2;
        float[] fluxThreshold = new float[rectifiedSpectralFlux.Length];
        float[] prunedSpectralFlux = new float[rectifiedSpectralFlux.Length];
        bool isPeak = false;

        for (int i = 0; i < rectifiedSpectralFlux.Length; i++)
        {
            // Generate threshold
            int windowStartIndex = Mathf.Max (0, i - windowSize);
            int windowEndIndex = Mathf.Min (rectifiedSpectralFlux.Length - 1, i + windowSize);
            
            // Add up our spectral flux over the window
            float sum = 0f;
            for (int j = windowStartIndex; j < windowEndIndex; j++)
                sum += rectifiedSpectralFlux[j];

            // The average multiplied by our sensitivity multiplier
            fluxThreshold[i] = sum / (windowEndIndex - windowStartIndex) * thresholdMultiplier;
            
            // Only keep the portion of the flux that is above the threshold
            prunedSpectralFlux[i] = Mathf.Max (0f, rectifiedSpectralFlux[i] - fluxThreshold[i]);

            // We do this by comparing the sample’s pruned spectral flux to its immediate neighbors
            // If it is higher than both the previously sampled pruned spectral flux, and the next, then it is a peak
            isPeak = i > 0 && i < rectifiedSpectralFlux.Length - 1 &&
                     prunedSpectralFlux[i] > prunedSpectralFlux[i + 1] &&
                     prunedSpectralFlux[i] > prunedSpectralFlux[i - 1];
            
            // Here we want to leave once we detect a peak
            if(isPeak)
                break;
        }
        
        peaks.Add(GetTimeFromIndex(frame), isPeak);
    }

    public float GetAnalyzeProgression()
    {
        return (float)currentSpectrumAnalyzed / (float)(spectrum.Length - 1);
    }

    public IReadOnlyDictionary<float, bool> GetPeaks()
    {
        return peaks;
    }

    public int GetSpectrumSampleSize()
    {
        return spectrumSampleSize;
    }

    private float GetTimeFromIndex(int index) {
        return 1f / sampleRate * index * spectrumSampleSize;
    }
}
