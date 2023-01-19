using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioAnalysis;

public class Tests : MonoBehaviour
{
    public AudioSource source;
    private float[] data;
    private bool first = true;

    void Start()
    {
        if (source)
        {
            data = new float[source.clip.samples * source.clip.channels];
            source.clip.GetData(data, 0);
            TestSections();
        }
        
        /*float[] a = { 1, 2, 3 };
        float[] b = { 4, 5, 6 };
        float[] c = Convolution.Convolve1D(a,b);
        Debug.Log(c);
        for(int i = 0; i < c.Length; i++)
        {
            Debug.Log(c[i]);
        }*/
    }

    public void TestSections()
    {
        //float[] fftSignal = AudioTools.FFT(signal, clip.samples, clip.channels, clip.frequency);
        float[] result = AudioTools.GetSectionsTimestamps(data, source.clip.frequency/120, source.clip.frequency);
        Debug.Log(data.Length);
        Debug.Log(result.Length);
        if (result.Length < 20)
        {
            foreach(float f in result)
            {
                Debug.Log(f);
            }
        }
    }
}
