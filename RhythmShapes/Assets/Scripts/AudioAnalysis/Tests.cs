using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioAnalysis;
using shape;
using models;
using utils.XML;

public class Tests : MonoBehaviour
{
    public AudioSource source;
    private float[] data;
    [SerializeField] private SpriteRenderer leftTargetColor;
    private bool first = true;

    void Start()
    {

        BPMGridShiftAndRounding();
        /*if (source)
        {
            data = new float[source.clip.samples * source.clip.channels];
            source.clip.GetData(data, 0);
            TestSections();
        }*/
        
        /*float[] a = { 1, 2, 3 };
        float[] b = { 4, 5, 6 };
        float[] c = Convolution.Convolve1D(a,b);
        Debug.Log(c);
        for(int i = 0; i < c.Length; i++)
        {
            Debug.Log(c[i]);
        }*/
    }

    public ShapeDescription[] getSampleData(float bpm)
    {
        List<ShapeDescription> shapes = new List<ShapeDescription>();

        Debug.Log("BPM:" + bpm);
        ShapeDescription shape = new ShapeDescription();
        //shape.SetModel(model);
        shapes.Add(shape);
        for (int i = 1; i < 52; i++)
        {
            float time = Random.Range(i * 60 / bpm, (i + 1) * 60 / bpm);
            shape = new ShapeDescription();
            shape.timeToPress = time;
            shape.type = (shape.ShapeType)Mathf.RoundToInt(Random.Range(0, 2));
            //shape.SetModel(model);
            //shape.SetTimeToPress(time);
            shapes.Add(shape);
        }

        ShapeDescription[] data = shapes.ToArray();
        return data;
    } 

    public void BPMGridShiftAndRounding()
    {
        float bpm = 120f;//Random.value * 100 + 100;
        ShapeDescription[] data = getSampleData(bpm);
        //data[0].SetTimeToPress(0.62134f);
        //data[5].SetTimeToPress(2.58f);
        Debug.Log(data.Length);
        Debug.Log(data[0].timeToPress);
        Debug.Log("5: "+data[5].timeToPress);
        Debug.Log("10: "+data[10].timeToPress);
        Debug.Log("15: "+data[20].timeToPress);
        float firstBeatTime = data[PostAnalysisTools.FirstBeatIndex(data, bpm)].timeToPress;
        Debug.Log("1st BEAT TIME: "+ firstBeatTime);

        PostAnalysisTools.snapNotesToBPMGrid(data, bpm, 1f, (float)1/6);

        Debug.Log(data[5].timeToPress);
        Debug.Log(data[10].timeToPress);
        Debug.Log(data[20].timeToPress);

        PostAnalysisTools.RoundTimingToMilliseconds(data);

        Debug.Log(data[5].timeToPress);
        Debug.Log(data[10].timeToPress);
        Debug.Log(data[20].timeToPress);
    }

    public void TestPatternRecognition()
    {

        float bpm = 120f;//Random.value * 100 + 100;
        ShapeDescription[] data = getSampleData(bpm);

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
