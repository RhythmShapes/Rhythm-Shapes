using System;
using System.IO;
using AudioAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using utils;

namespace edition
{
    public class EditorPanel : MonoBehaviour
    {
        [Header("Panel components")]
        [Space]
        [SerializeField] private GameObject useLevelMusicObject;
        [SerializeField] private GameObject musicPathObject;
        [SerializeField] private TMP_InputField musicPathField;
        [SerializeField] private TMP_InputField levelNameField;
        [SerializeField] private RangeField minimalNoteDelayField;
        [SerializeField] private RangeField peakThresholdField;
        [SerializeField] private Button analyseButton;
        
        [Space]
        [Header("Display messages")]
        [Space]
        [SerializeField] private ErrorMessage musicPathError;
        [SerializeField] private ErrorMessage levelNameError;
        [SerializeField] private PopupWindow popupWindow;
        [Space]
        [SerializeField] private UnityEvent<string> onRequestAnalysis;

        private static EditorPanel _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;

            onRequestAnalysis ??= new UnityEvent<string>();
        }

        public static void Init()
        {
            EditorModel.UseLevelMusic = !GameInfo.IsNewLevel;
            _instance.useLevelMusicObject.SetActive(EditorModel.UseLevelMusic);
            _instance.musicPathObject.SetActive(GameInfo.IsNewLevel);
            _instance.minimalNoteDelayField.SetValueWithoutNotify(MultiRangeAnalysis.minimalNoteDelay);
            _instance.peakThresholdField.SetValueWithoutNotify(MultiRangeAnalysis.analysisThreshold);
        }

        public void SetActive(bool active)
        {
            if (!GameInfo.IsNewLevel && string.IsNullOrEmpty(levelNameField.text))
            {
                levelNameField.SetTextWithoutNotify(EditorModel.OriginLevel.title);
                OnSetLevelName(EditorModel.OriginLevel.title);
                minimalNoteDelayField.SetValueWithoutNotify(MultiRangeAnalysis.minimalNoteDelay);
                peakThresholdField.SetValueWithoutNotify(MultiRangeAnalysis.analysisThreshold);
            }

            CheckFields();
            gameObject.SetActive(active);
        }

        public static void RequestAnalysis()
        {
            _instance.OnSetRequestAnalysis();
        }

        public void OnSetRequestAnalysis()
        {
            string sourceAudioPath = !GameInfo.IsNewLevel && EditorModel.UseLevelMusic
                ? LevelTools.GetLevelAudioFilePath(EditorModel.OriginLevel.title)
                : musicPathField.text;
            analyseButton.interactable = false;

            if (!CheckMusicPath(sourceAudioPath))
            {
                NotificationsManager.ShowError("An error in the music path prevents starting the analysis. Please fix it and try again.");
                analyseButton.interactable = true;
                return;
            }

            EditorModel.AnalyzedMusicPath = sourceAudioPath;
            onRequestAnalysis.Invoke(sourceAudioPath);
        }

        public void OnSetUseLevelMusic(bool use)
        {
            EditorModel.UseLevelMusic = use;
            musicPathObject.SetActive(!use);

            if (!use)
                CheckMusicPath(EditorModel.MusicPath);
        }

        public void OnSetMusicPath(string path)
        {
            EditorModel.MusicPath = path;
            
            if (!CheckMusicPath(path))
                return;

            if (musicPathField.text.Length > 0 && levelNameField.text.Length == 0)
            {
                string levelName = Path.GetFileNameWithoutExtension(path);
                levelNameField.text = levelName;
                OnSetLevelName(levelName);
            }
        }

        public void OnSetLevelName(string levelName)
        {
            EditorModel.LevelName = levelName;
            CheckLevelName(levelName);
        }

        public void OnSetMinimalNoteDelay(float delay)
        {
            MultiRangeAnalysis.minimalNoteDelay = delay;
        }

        public void OnSetPeakThreshold(float threshold)
        {
            MultiRangeAnalysis.analysisThreshold = threshold;
        }

        public static bool CheckFields()
        {
            return _instance.CheckLevelName(EditorModel.LevelName) && _instance.CheckMusicPath(EditorModel.MusicPath);
        }

        private bool CheckMusicPath(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                if (!GameInfo.IsNewLevel && EditorModel.UseLevelMusic)
                    return true;
                
                musicPathError.ShowError("Field cannot be empty.");
                return false;

            }
            
            if (!LevelTools.IsFilePathValid(path))
            {
                musicPathError.ShowError("Could not find music file.");
                return false;
            }

            return true;
        }

        private bool CheckLevelName(string levelName)
        {
            if(string.IsNullOrEmpty(levelName))
            {
                levelNameError.ShowError("Field cannot be empty.");
                return false;
            }
            
            if (((!GameInfo.IsNewLevel && !levelName.Equals(EditorModel.OriginLevel.title)) || GameInfo.IsNewLevel) && LevelTools.DoLevelExists(levelName))
            {
                levelNameError.ShowError("This name already exists.");
                return false;
            }

            return true;
        }
    }
}
