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
    
    // [SerializeField] private UnityEvent<float> onCalibrationCalculated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        // onCalibrationCalculated ??= new UnityEvent<float>();

        
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
                Debug.Log("TestingCalibration -> CalculateMean 1, diffI : "+ diffI);
                total += diffI;
            }

            calibration = total / count;
            Debug.Log("TestingCalibration -> CalculateMean 1, calibration : "+ total / count);
            // onCalibrationCalculated.Invoke(total / count);

        }
        else
        {
            if (inputReceivedTimeQueue.Count > shapeTheoricalPressTimeQueue.Count)
            {
                int count = inputReceivedTimeQueue.Count;
                float total = 0;
                float diffI = 0;
                for (int i = 0; i < count; i++)
                {
                    if (Mathf.Abs(inputReceivedTimeQueue.Peek() - shapeTheoricalPressTimeQueue.Peek()) > 1.5f)
                    {
                        inputReceivedTimeQueue.Dequeue();
                    }
                    else
                    {
                        diffI = inputReceivedTimeQueue.Dequeue() - shapeTheoricalPressTimeQueue.Dequeue();
                        Debug.Log("TestingCalibration -> CalculateMean 2, diffI : "+ diffI);
                        total += diffI;
                    }
                    
                }

                calibration = total / count;
                Debug.Log("TestingCalibration -> CalculateMean 2, calibration : "+ total / count);
                // onCalibrationCalculated.Invoke(total / count);
            }
            else if (inputReceivedTimeQueue.Count > shapeTheoricalPressTimeQueue.Count)
            {
                int count = shapeTheoricalPressTimeQueue.Count;
                float total = 0;
                float diffI = 0;
                for (int i = 0; i < count; i++)
                {
                    if (Mathf.Abs(inputReceivedTimeQueue.Peek() - shapeTheoricalPressTimeQueue.Peek()) > 1.5f)
                    {
                        shapeTheoricalPressTimeQueue.Dequeue();
                    }
                    else
                    {
                        diffI = inputReceivedTimeQueue.Dequeue() - shapeTheoricalPressTimeQueue.Dequeue();
                        Debug.Log("TestingCalibration -> CalculateMean 3, diffI : "+ diffI);
                        total += diffI;
                    }
                    
                }

                calibration = total / count;
                Debug.Log("TestingCalibration -> CalculateMean 3, calibration : "+ total / count);
                // onCalibrationCalculated.Invoke(total / count);
            }
        }
        
    }
}
