using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using ui;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using utils;
using utils.XML;

public class MusicSelectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI numberOfNotesTextTMP;
    [SerializeField] private TextMeshProUGUI numberOfNotesPerSecondTextTMP;
    [SerializeField] private TextMeshProUGUI minimalNoteDelayTextTMP;
    [SerializeField] private TextMeshProUGUI analysisThresholdTextTMP;
    [SerializeField] private TextMeshProUGUI doubleNoteAnalysisThresholdTextTMP;
    public static MusicSelectionManager Instance { get; private set; }

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    private void Start()
    {
        foreach (string levelName in Directory.GetDirectories(Path.Combine(Application.persistentDataPath, "Levels")))
        {
            Instantiate(buttonPrefab, content).GetComponent<MenuButton>().Init(Path.GetFileName(levelName), LevelTools.LoadLevelData(levelName));
        }
    }


    public void SetLevelDifficultyText (int nbNotes, float nbNotesPerSec, float minNoteDelay, float analysisThreshold, float doubleNoteAnalysisThreshold)
    {
        if (numberOfNotesTextTMP != null && 
            numberOfNotesPerSecondTextTMP != null && 
            minimalNoteDelayTextTMP != null && 
            analysisThresholdTextTMP != null && 
            doubleNoteAnalysisThresholdTextTMP != null)
        {
            numberOfNotesTextTMP.text = nbNotes.ToString();
            numberOfNotesPerSecondTextTMP.text = nbNotesPerSec.ToString("F3");
            minimalNoteDelayTextTMP.text = minNoteDelay.ToString("F3");
            analysisThresholdTextTMP.text = analysisThreshold.ToString("F3");
            doubleNoteAnalysisThresholdTextTMP.text = doubleNoteAnalysisThreshold.ToString("F3");
            return;
        }
        
        Debug.LogError("At least One Text is not set");
        
    }
}
