using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using utils;
using utils.XML;

namespace edition
{
    public class EditorPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField sourceAudioPathField;
        [SerializeField] private TMP_InputField levelNameField;
        [SerializeField] private Button analyseButton;
        [SerializeField] private PopupWindow popupWindow;
        [SerializeField] private UnityEvent<string> onRequestAnalysis;

        public static EditorPanel Instance { get; private set; }

        private LevelDescription _level;
        private string _savedLevelName;
        private string _savedSourceAudioPath;
        private ErrorMessage _sourceAudioPathError;
        private ErrorMessage _levelNameError;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            _sourceAudioPathError = sourceAudioPathField.GetComponentInParent<ErrorMessage>();
            _levelNameError = levelNameField.GetComponentInParent<ErrorMessage>();
            popupWindow = FindObjectOfType<PopupWindow>();
            onRequestAnalysis ??= new UnityEvent<string>();
        }

        private void FixedUpdate()
        {
            analyseButton.enabled = sourceAudioPathField.text.Length > 0 && levelNameField.text.Length > 0;
        }

        public void SetLevel(LevelDescription level)
        {
            _level = level;

            if (string.IsNullOrEmpty(_savedLevelName) && IsLevelDefined())
                SetSavedLevelName(_level.title);
            
            if (string.IsNullOrEmpty(_savedSourceAudioPath) && IsLevelDefined() && LevelTools.DoLevelContainsAudio(_level.title))
                SetSavedSourceAudioPath(LevelTools.GetLevelAudioFilePath(_level.title));

            if (IsLevelDefined() && levelNameField.text.Length == 0)
            {
                levelNameField.SetTextWithoutNotify(_level.title);
                CheckLevelName(_level.title);
            }
        }

        public void SetSavedLevelName(string levelName)
        {
            _savedLevelName = levelName;
        }

        public void SetSavedSourceAudioPath(string path)
        {
            _savedSourceAudioPath = path;
        }

        public bool IsLevelDefined()
        {
            return _level != null;
        }

        public bool IsLevelNameDefined()
        {
            return !string.IsNullOrEmpty(GetLevelName());
        }

        public bool IsSourceAudioPathDefined()
        {
            return !string.IsNullOrEmpty(GetSourceAudioPath());
        }

        public LevelDescription GetLevel()
        {
            return _level;
        }

        public string GetLevelName()
        {
            return levelNameField.text.Trim();
        }

        public string GetSourceAudioPath()
        {
            return sourceAudioPathField.text.Trim();
        }

        public void SetActive(bool active)
        {
            if (!IsLevelDefined())
            {
                sourceAudioPathField.SetTextWithoutNotify(string.Empty);
                levelNameField.SetTextWithoutNotify(_level.title);
                CheckLevelName(_level.title);
            }

            gameObject.SetActive(active);
        }

        public void OnSetRequestAnalysis()
        {
            string sourceAudioPath = sourceAudioPathField.text;
            string levelName = levelNameField.text;
            analyseButton.enabled = false;

            if (!CheckSourceAudioPath(sourceAudioPath))
            {
                if(popupWindow != null)
                    popupWindow.ShowError("An error in the source audio path prevents starting the analysis. Please fix it and try again.");
                analyseButton.enabled = true;
                return;
            }

            /*
            // Create folder
            if (!IsLevelDefined())
            {
                LevelTools.CreateLevelFolder(levelName);
                if (!LevelTools.DoLevelExists(levelName))
                {
                    if(popupWindow != null)
                        popupWindow.ShowError("An error occurs when trying to create level files.");
                    analyseButton.enabled = true;
                    return;
                }
            }
            
            // Rename folder
            if (IsLevelDefined() && !levelName.Equals(_level.title))
            {
                LevelTools.CreateLevelFolder(levelName);
                if (!LevelTools.DoLevelExists(levelName))
                {
                    if(popupWindow != null)
                        popupWindow.ShowError("An error occurs when trying to create level files.");
                    analyseButton.enabled = true;
                    return;
                }
            }
            
            // Set audio
            // Replace audio
            
            if (!IsLevelDefined() || !levelName.Equals(_level.title))
            {
                LevelTools.CreateLevelFolder(levelName);
                if (!LevelTools.DoLevelExists(levelName))
                {
                    if(popupWindow != null)
                        popupWindow.ShowError("Could not save level.");
                    Debug.LogError("Could not create level folder " + LevelTools.GetLevelFolderPath(levelName));
                    analyseButton.enabled = true;
                    return;
                }
            }

            LevelTools.CopyLevelAudioFile(levelName, sourceAudioPath);
            if (!LevelTools.DoLevelContainsAudio(levelName))
            {
                if(popupWindow != null)
                    popupWindow.ShowError("Could not save level.");
                Debug.LogError("Could not copy audio file " + sourceAudioPath + " into " + LevelTools.GetLevelAudioFilePath(levelName));
                analyseButton.enabled = true;
                return;
            }*/
            
            onRequestAnalysis.Invoke(sourceAudioPath);
        }

        public void OnSetSourceAudioPath(string path)
        {
            if (!CheckSourceAudioPath(path))
                return;

            if (sourceAudioPathField.text.Length > 0 && levelNameField.text.Length == 0)
            {
                levelNameField.text = Path.GetFileNameWithoutExtension(path);
                CheckLevelName(levelNameField.text);
            }
        }

        public void OnSetLevelName(string levelName)
        {
            CheckLevelName(levelName);
        }

        private bool CheckSourceAudioPath(string path)
        {
            if(path.Length == 0)
                return false;
            
            if (!File.Exists(path))
            {
                _sourceAudioPathError.ShowError("Could not find audio file");
                return false;
            }
            
            if (!string.IsNullOrEmpty(_savedSourceAudioPath) && !_savedSourceAudioPath.Equals(path))
                _sourceAudioPathError.ShowError("The level music will be overriden by this one if you save");

            return true;
        }

        private bool CheckLevelName(string levelName)
        {
            if(levelName.Length == 0)
                return false;
            
            if (((IsLevelDefined() && !levelName.Equals(_level.title)) || !IsLevelDefined()) && LevelTools.DoLevelExists(levelName))
            {
                _levelNameError.ShowError("This name already exists");
                return false;
            }
            
            if (!string.IsNullOrEmpty(_savedLevelName) && !_savedLevelName.Equals(levelName))
                _levelNameError.ShowError("The level name will be overriden by this one if you save");

            return true;
        }
    }
}
