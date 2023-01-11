using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class TestingCalibration : MonoBehaviour
{
    public static TestingCalibration Instance { get; private set; }
    public float calibration = 0.07372008f; //0,066417 //0,127751 //
    public Queue<float> inputReceivedTimeQueue;
    public Queue<float> shapeTheoricalPressTimeQueue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        
    }

    private void OnEnable()
    {
        inputReceivedTimeQueue = new Queue<float>();
        shapeTheoricalPressTimeQueue = new Queue<float>();
    }

    private void OnDisable()
    {
        inputReceivedTimeQueue.Clear();
        shapeTheoricalPressTimeQueue.Clear();
    }

    public void CalculateMean()
    {
        if (inputReceivedTimeQueue.Count == shapeTheoricalPressTimeQueue.Count)
        {
            int count = inputReceivedTimeQueue.Count;
            float total = 0;
            float diffI = 0;
            for (int i = 0; i < count; i++)
            {
                diffI = inputReceivedTimeQueue.Dequeue() - shapeTheoricalPressTimeQueue.Dequeue();
                Debug.Log("TestingCalibration -> CalculateMean, diffI : "+ diffI);
                total += diffI;
            }

            // calibration = total / count;
            Debug.Log("TestingCalibration -> CalculateMean, calibration : "+ total / count);

        }
        else
        {
            Debug.LogError("Differente Size Queue");
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
