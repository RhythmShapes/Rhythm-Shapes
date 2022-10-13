using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioAnalyzer2))]
public class AudioShow : MonoBehaviour
{
    [SerializeField] private Transform peakShow;
    [SerializeField] private float peakShowReduceRate = .05f;
    private AudioSource audioSource;
    private AudioAnalyzer2 audioAnalyzer;
    private IReadOnlyDictionary<float, bool> peaks;
    private float[] times;
    private float lengthPerSample;
    private int clipSamples;
    private int spectrumSampleSize;
    private Vector3 scale = Vector3.zero;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioAnalyzer = GetComponent<AudioAnalyzer2>();
        lengthPerSample = audioSource.clip.length / audioSource.clip.samples;
        spectrumSampleSize = audioAnalyzer.GetSpectrumSampleSize();
    }

    private void Update()
    {
        int index = Mathf.FloorToInt (audioSource.time / lengthPerSample) / spectrumSampleSize;
        if(index >= times.Length)
            return;
        
        float time = times[index];
        scale.x = Mathf.Max(0, scale.x - peakShowReduceRate);
        scale.y = Mathf.Max(0, scale.y - peakShowReduceRate);
        
        if (peaks[time])
            scale = new Vector3(10, 10, 0);
        
        peakShow.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (peaks == null && audioAnalyzer.isReady)
        {
            peaks = audioAnalyzer.GetPeaks();
            times = new float[peaks.Count];
            
            int i = 0;
            foreach (KeyValuePair<float, bool> item in peaks)
            {
                times[i] = item.Key;
                i++;
            }
            
            audioSource.volume = .1f;
            audioSource.Play();
        }
    }
}