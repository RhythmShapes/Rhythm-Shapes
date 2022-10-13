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
public class AudioAnalyzer : MonoBehaviour
{
    // Spectrum chunk size
    [SerializeField] [Range(64, 8192)] private int spectrumSampleSize = 1024;
    
    // Sensitivity multiplier to scale the average threshold.
    // In this case, if a rectified spectral flux sample is > 1.5 times the average, it is a peak
    [SerializeField] [Min(0)] private float thresholdMultiplier = 1.5f;
    
    // Number of samples (window) to average in flux threshold calculations
    [SerializeField] [Min(0)] private int thresholdWindowSize = 50;

    private AudioSource audioSource;
    
    private void OnValidate() {
        spectrumSampleSize = Mathf.ClosestPowerOfTwo(spectrumSampleSize);
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Debug.Log(GetAnalyzeProgression() * 100);
    }

    public float GetAnalyzeProgression()
    {
        return audioSource.time / audioSource.clip.length;
    }
}
