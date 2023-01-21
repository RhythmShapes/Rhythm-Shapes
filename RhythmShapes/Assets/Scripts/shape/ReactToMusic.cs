using AudioAnalysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToMusic : MonoBehaviour
{
    public AudioSource source;
    private float[] signal = new float[2048];
    private float signalLevel;
    private float oldSignalLevel;
    private float maxSignalLevel = 1;

    private void Start()
    {
        source = GameObject.Find("GameplayManager").GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        source.GetSpectrumData(signal,0, FFTWindow.Rectangular);
        signal = AudioTools.Normalize(signal);
        signalLevel = 0;
        for(int i = 0; i < signal.Length; i++)
        {
            signalLevel += signal[i];
        }

        if(signalLevel < oldSignalLevel)
        {
            signalLevel = 0.9f*oldSignalLevel;
        }

        if (signalLevel > maxSignalLevel)
        {
            maxSignalLevel = signalLevel;
        }
        signalLevel = signalLevel / maxSignalLevel + 0.8f;
        if (!float.IsNaN(signalLevel))
        {
            transform.localScale = new Vector3(signalLevel, signalLevel, signalLevel);
        }
        oldSignalLevel = signalLevel;
    }
}
