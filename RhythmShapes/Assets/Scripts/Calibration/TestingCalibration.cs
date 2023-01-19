using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;
using Input = UnityEngine.Windows.Input;

public class TestingCalibration : MonoBehaviour
{
    public static TestingCalibration Instance { get; private set; }
    public float calibration; //0,066417 //0,127751 //
    private float gameCalibration;
    private Queue<float> inputReceivedTimeQueue = new();
    private Queue<float> shapeTheoricalPressTimeQueue = new();
    private float[] shapeTheoricalPressTime;
    
    // [SerializeField] private UnityEvent<float> onCalibrationCalculated;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        calibration = 0;
        gameCalibration = GameInfo.Calibration;
        GameInfo.Calibration = 0;
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
        if (inputReceivedTimeQueue.Count == 0 || shapeTheoricalPressTimeQueue.Count == 0)
        {
            // Debug.Log("Count == 0");
            if (gameCalibration < -0.05)
            {
                calibration = -0.05f;
            }
            else if (gameCalibration > 0.2f)
            {
                calibration = 0.2f;
            }
            else
            {
                calibration = gameCalibration;
            }
            // GameInfo.Calibration = calibration;
            // PlayerPrefsManager.SetPref("InputOffset",calibration);
        }
        else
        {
            // Debug.Log("Count != 0");
            if (inputReceivedTimeQueue.Count == shapeTheoricalPressTimeQueue.Count)
            {
                // Debug.Log("inputCount == shapeCount");
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
                if (calibration < -0.05f)
                {
                    calibration = -0.05f;
                }
                else if (calibration > 0.2f)
                {
                    calibration = 0.2f;
                }
                
                // GameInfo.Calibration = calibration;
                // Debug.Log("TestingCalibration -> CalculateMean 1, calibration : "+ total / count);
                // PlayerPrefsManager.SetPref("InputOffset",calibration);
                // onCalibrationCalculated.Invoke(total / count);

            }
            else
            {
                if (inputReceivedTimeQueue.Count > shapeTheoricalPressTimeQueue.Count)
                {
                    Debug.Log("inputCount > shapeCount");
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
                    if (calibration < -0.05)
                    {
                        calibration = -0.05f;
                    }
                    else if (calibration > 0.2f)
                    {
                        calibration = 0.2f;
                    }
                    // GameInfo.Calibration = calibration;
                    // Debug.Log("TestingCalibration -> CalculateMean 2, calibration : "+ total / count);
                    // PlayerPrefsManager.SetPref("InputOffset",calibration);
                    // onCalibrationCalculated.Invoke(total / count);
                }
                else if (inputReceivedTimeQueue.Count < shapeTheoricalPressTimeQueue.Count)
                {
                    Debug.Log("inputCount < shapeCount");
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
                    if (calibration < -0.05f)
                    {
                        calibration = -0.05f;
                    }
                    else if (calibration > 0.2f)
                    {
                        calibration = 0.2f;
                    }
                    // GameInfo.Calibration = calibration;
                    // Debug.Log("TestingCalibration -> CalculateMean 3, calibration : "+ total / count);
                    // PlayerPrefsManager.SetPref("InputOffset",calibration);
                    // onCalibrationCalculated.Invoke(total / count);
                }
            }
        }
        
    }

    private void Update()
    {
        // Debug.Log("TestingCalibration -> Update, count : " + inputReceivedTimeQueue.Count + ", " +shapeTheoricalPressTimeQueue.Count);
    }

    public void EnqueueInputReceivedTimeQueue(float value)
    {
        // Debug.Log("Testing Calibration -> EnqueueInputReceivedTimeQueue audioSource.time : "+ _audioSource.time);
        // inputReceivedTimeQueue.Enqueue(_audioSource.time);
        // Debug.Log("Testing Calibration -> EnqueueInputReceivedTimeQueue audioSource.time : "+value);
        inputReceivedTimeQueue.Enqueue(value);
    }
    
    public void EnqueueShapeTheoricalPressTimeQueue(float value)
    {
        // Debug.Log("EnqueueShapeTheoricalPressTimeQueue : "+ value);
        shapeTheoricalPressTimeQueue.Enqueue(value);
    }

    public void ResetAll()
    {
        inputReceivedTimeQueue.Clear();
        shapeTheoricalPressTimeQueue.Clear();
        calibration = 0;
    }

    public void SaveCalibration()
    {
        GameInfo.Calibration = calibration;
        PlayerPrefsManager.SetPref("InputOffset",calibration);
    }

    public void SaveOldCalibration()
    {
        GameInfo.Calibration = gameCalibration;
        PlayerPrefsManager.SetPref("InputOffset",gameCalibration);
    }
}
