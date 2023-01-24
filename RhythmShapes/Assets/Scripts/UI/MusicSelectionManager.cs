using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using ui;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using utils;
using utils.XML;

public class MusicSelectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI numberOfNotesTextTMP;
    [SerializeField] private TextMeshProUGUI audioLengthTextTMP;
    [SerializeField] private TextMeshProUGUI numberOfNotesPerSecondTextTMP;
    [SerializeField] private TextMeshProUGUI minimalNoteDelayTextTMP;
    [SerializeField] private TextMeshProUGUI numberOfDoubleNotesTextTMP;
    
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


    public void SetLevelDifficultyText (int nbNotes, float audioLength, float nbNotesPerSec, float minNoteDelay, int nbDoubleNotes)
    {
        numberOfNotesTextTMP.text = nbNotes.ToString();
        var minutes = audioLength / 60;
        var seconds = audioLength % 60;
        audioLengthTextTMP.text = Mathf.Floor(minutes)+" m "+ Mathf.RoundToInt(seconds )+" s";
        numberOfNotesPerSecondTextTMP.text = nbNotesPerSec.ToString("F3");
        minimalNoteDelayTextTMP.text = minNoteDelay.ToString("F2");
        numberOfDoubleNotesTextTMP.text = nbDoubleNotes.ToString();
        
    }
}
