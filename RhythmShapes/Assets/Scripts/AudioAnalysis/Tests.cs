using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioAnalysis;
using shape;
using models;

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

    public Shape[] getSampleData(float bpm)
    {
        List<Shape> shapes = new List<Shape>();

        Debug.Log("BPM:" + bpm);
        Vector2[] path = { new Vector2(0, 0), new Vector2(1, 1) };
        Shape shape = new Shape();
        ShapeModel model = new ShapeModel(ShapeType.Square, Target.Top, leftTargetColor.color, path, 1f, 0.15f, 1f);
        //shape.SetModel(model);
        shapes.Add(shape);
        for (int i = 1; i < 52; i++)
        {
            float time = Random.Range(i * 60 / bpm, (i + 1) * 60 / bpm);
            shape = new Shape();
            model = new ShapeModel(ShapeType.Square, Target.Top, leftTargetColor.color, path, 1f, 0.15f, 1f);
            //shape.SetModel(model);
            //shape.SetTimeToPress(time);
            shapes.Add(shape);
        }

        Shape[] data = shapes.ToArray();
        return data;
    } 

    public void BPMGridShiftAndRounding()
    {
        float bpm = 120f;//Random.value * 100 + 100;
        Shape[] data = getSampleData(bpm);
        //data[0].SetTimeToPress(0.62134f);
        //data[5].SetTimeToPress(2.58f);
        Debug.Log(data.Length);
        Debug.Log(data[0].TimeToPress);
        Debug.Log("5: "+data[5].TimeToPress);
        Debug.Log("10: "+data[10].TimeToPress);
        Debug.Log("15: "+data[20].TimeToPress);
        float firstBeatTime = data[PostAnalysisTools.FirstBeatIndex(data, bpm)].TimeToPress;
        Debug.Log("1st BEAT TIME: "+ firstBeatTime);

        PostAnalysisTools.snapNotesToBPMGrid(data, bpm, 1f, (float)1/6);

        Debug.Log(data[5].TimeToPress);
        Debug.Log(data[10].TimeToPress);
        Debug.Log(data[20].TimeToPress);

        PostAnalysisTools.RoundTimingToMilliseconds(data);

        Debug.Log(data[5].TimeToPress);
        Debug.Log(data[10].TimeToPress);
        Debug.Log(data[20].TimeToPress);
    }

    public void TestPatternRecognition()
    {

        float bpm = 120f;//Random.value * 100 + 100;
        Shape[] data = getSampleData(bpm);

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
