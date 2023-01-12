using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;
using Input = UnityEngine.Windows.Input;

public class TestingCalibration : MonoBehaviour
{
    public static TestingCalibration Instance { get; private set; }
    [SerializeField] private float calibration; //0,066417 //0,127751 //
    private Queue<float> inputReceivedTimeQueue = new();
    private Queue<float> shapeTheoricalPressTimeQueue = new();
    [SerializeField] private AudioSource _audioSource;
    
    
    // [SerializeField] private UnityEvent<float> onCalibrationCalculated;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        calibration = GameInfo.Calibration;
        gameObject.GetComponent<TestingCalibration>().enabled = true;
        // onCalibrationCalculated ??= new UnityEvent<float>();
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
                // Debug.Log("TestingCalibration -> CalculateMean 1, diffI : "+ diffI);
                total += diffI;
            }

            calibration = total / count;
            GameInfo.Calibration = calibration;
            // Debug.Log("TestingCalibration -> CalculateMean 1, calibration : "+ total / count);
            PlayerPrefsManager.Instance.SetPref("InputOffset",calibration);
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
                        // Debug.Log("TestingCalibration -> CalculateMean 2, diffI : "+ diffI);
                        total += diffI;
                    }
                    
                }

                calibration = total / count;
                GameInfo.Calibration = calibration;
                // Debug.Log("TestingCalibration -> CalculateMean 2, calibration : "+ total / count);
                PlayerPrefsManager.Instance.SetPref("InputOffset",calibration);
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
                        // Debug.Log("TestingCalibration -> CalculateMean 3, diffI : "+ diffI);
                        total += diffI;
                    }
                    
                }

                calibration = total / count;
                GameInfo.Calibration = calibration;
                // Debug.Log("TestingCalibration -> CalculateMean 3, calibration : "+ total / count);
                PlayerPrefsManager.Instance.SetPref("InputOffset",calibration);
                // onCalibrationCalculated.Invoke(total / count);
            }
        }
    }

    private void Update()
    {
        // Debug.Log("TestingCalibration -> Update 3, count : " + inputReceivedTimeQueue.Count + ", " +shapeTheoricalPressTimeQueue.Count);
    }

    public void EnqueueInputReceivedTimeQueue()
    {
        // Debug.Log("EnqueueShapeTheoricalPressTimeQueue : "+ _audioSource.time);
        inputReceivedTimeQueue.Enqueue(_audioSource.time);
    }
    
    public void EnqueueShapeTheoricalPressTimeQueue(float value)
    {
        // Debug.Log("EnqueueShapeTheoricalPressTimeQueue : "+ value);
        shapeTheoricalPressTimeQueue.Enqueue(value);
    }

    public void ClearQueue()
    {
        inputReceivedTimeQueue.Clear();
        shapeTheoricalPressTimeQueue.Clear();
    }
}
