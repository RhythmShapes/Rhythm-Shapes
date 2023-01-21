using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AudioAnalysis;
using edition.panels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using utils;

public class PresetDifficulty : MonoBehaviour
{
    [Header("RangeField")]
    [Space]
    [SerializeField] private RangeField minimalNoteDelayField;
    [SerializeField] private RangeField peakThresholdField;
    [SerializeField] private RangeField doubleNoteThresholdField;
    
    [Space]
    [Header("Sliders")]
    [Space]
    [SerializeField] private Slider analysisMinimalDelaySlider;
    [SerializeField] private Slider analysisPeakThresholdSlider;
    [SerializeField] private Slider analysisDoubleNoteThresholdSlider;
    
    [Space]
    [Header("EasyPreset")]
    [Space]
    [SerializeField] private float easyMinimalDelay = 0.5f;
    [SerializeField] private float easyPeakThreshold = 0.4f;
    [SerializeField] private float easyDoubleNoteThreshold = 5;
    
    [Space]
    [Header("MeduimPreset")]
    [Space]
    [SerializeField] private float mediumMinimalDelay = 0.2f;
    [SerializeField] private float mediumPeakThreshold = 0.3f;
    [SerializeField] private float mediumDoubleNoteThreshold = 5;
    
    [Space]
    [Header("HardPreset")]
    [Space]
    [SerializeField] private float hardMinimalDelay = 0.1f;
    [SerializeField] private float hardPeakThreshold = 0.3f;
    [SerializeField] private float hardDoubleNoteThreshold = 0.4f;

    [Space]
    [Header("Music")]
    [Space]
    public string musicPath;
    [SerializeField] private string levelName;
    [SerializeField] private UnityEvent onRequestAnalysis;
    
    [Space]
    [Header("UI")]
    [Space]
    [SerializeField] private UnityEvent onNewButtonSelected;
    
    private bool firstSet = true;
    
    public static PresetDifficulty Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        onRequestAnalysis ??= new UnityEvent();
        onNewButtonSelected ??= new UnityEvent();
        
    }

    public void Init()
    {
        firstSet = true;
    }

    public void SetBaseValue()
    {
        minimalNoteDelayField.SetValueWithoutNotify(MultiRangeAnalysis.minimalNoteDelay);
        peakThresholdField.SetValueWithoutNotify(MultiRangeAnalysis.analysisThreshold);
        doubleNoteThresholdField.SetValueWithoutNotify(MultiRangeAnalysis.doubleNoteAnalysisThreshold);
    }

    public void SetEasyPreset()
    {
        SetMinimalDelay(easyMinimalDelay);
        minimalNoteDelayField.SetValueWithoutNotify(easyMinimalDelay);
        SetPeakThreshold(easyPeakThreshold);
        peakThresholdField.SetValueWithoutNotify(easyPeakThreshold);
        SetDoubleNoteThreshold(easyDoubleNoteThreshold);
    }
    
    public void SetMediumPreset()
    {
        SetMinimalDelay(mediumMinimalDelay);
        minimalNoteDelayField.SetValueWithoutNotify(mediumMinimalDelay);
        SetPeakThreshold(mediumPeakThreshold);
        peakThresholdField.SetValueWithoutNotify(mediumPeakThreshold);
        SetDoubleNoteThreshold(mediumDoubleNoteThreshold);
    }
    
    public void SetHardPreset()
    {
        SetMinimalDelay(hardMinimalDelay);
        minimalNoteDelayField.SetValueWithoutNotify(hardMinimalDelay);
        SetPeakThreshold(hardPeakThreshold);
        peakThresholdField.SetValueWithoutNotify(hardPeakThreshold);
        SetDoubleNoteThreshold(hardDoubleNoteThreshold);
    }

    public void SetMinimalDelay(float value)
    {
        MultiRangeAnalysis.minimalNoteDelay = value;
    }
    
    public void SetPeakThreshold(float value)
    {
        MultiRangeAnalysis.analysisThreshold = value;
    }
    
    public void SetDoubleNoteThreshold(float value)
    {
        value = Mathf.Max(value, MultiRangeAnalysis.analysisThreshold);
        MultiRangeAnalysis.doubleNoteAnalysisThreshold = value;
        doubleNoteThresholdField.SetValueWithoutNotify(value);

    }

    public void RequestAnalysis()
    {
        GameInfo.LevelName = levelName;
        GameInfo.RequestAnalysis = true;
        GameInfo.IsNewLevel = true;
        onRequestAnalysis.Invoke();
    }
    
    public void OnSetMusicPath(string path)
    {
        musicPath = path;
            
        if (!CheckMusicPath(path))
            return;

        if (musicPath.Length > 0)
        {
            string lvlName = Path.GetFileNameWithoutExtension(path);
            levelName = lvlName;
            if (!CheckLevelName(levelName))
                return;

            RequestAnalysis();
        }
    }
    
    private bool CheckMusicPath(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            Debug.LogError("path cannot be empty.");
            return false;

        }
            
        if (!LevelTools.IsFilePathValid(path))
        {
            Debug.LogError("Could not find music file.");
            return false;
        }

        return true;
    }

    private bool CheckLevelName(string levelName)
    {
        if(string.IsNullOrEmpty(levelName))
        {
            Debug.LogError("levelName cannot be empty.");
            return false;
        }
            
        if (LevelTools.DoLevelExists(levelName))
        {
            Debug.LogError("This name already exists.");
            return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        if (firstSet)
        {
            firstSet = false;
            onNewButtonSelected.Invoke();
        }
    }
}
