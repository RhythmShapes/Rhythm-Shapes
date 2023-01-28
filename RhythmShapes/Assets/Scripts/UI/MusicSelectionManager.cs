using System.IO;
using TMPro;
using ui;
using UnityEngine;
using utils;

namespace UI
{
    public class MusicSelectionManager : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject difficultyContent;
        [SerializeField] private TextMeshProUGUI numberOfNotesTextTMP;
        [SerializeField] private TextMeshProUGUI audioLengthTextTMP;
        [SerializeField] private TextMeshProUGUI numberOfNotesPerSecondTextTMP;
        [SerializeField] private TextMeshProUGUI minimalNoteDelayTextTMP;
        [SerializeField] private TextMeshProUGUI numberOfDoubleNotesTextTMP;
        [SerializeField] private TextMeshProUGUI bestScoreTextTMP;
        [SerializeField] private TextMeshProUGUI bestComboTextTMP;
        [SerializeField] private bool launchEditor;
    
        public static MusicSelectionManager Instance { get; private set; }

        private string _currentLevelName;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        private void Start()
        {
            SetList();
        }

        public void SetList()
        {
            for (int i = 0; i < content.childCount; i++)
                Destroy(content.GetChild(i).gameObject);

            foreach (string levelName in Directory.GetDirectories(Path.Combine(Application.persistentDataPath, "Levels")))
            {
                Instantiate(buttonPrefab, content).GetComponent<MenuButton>().Init(Path.GetFileName(levelName), LevelTools.LoadLevelData(levelName), launchEditor);
            }
            difficultyContent.SetActive(false);
        }

        public void SetLevelDifficultyText (string levelName, int nbNotes, float audioLength, float nbNotesPerSec, float minNoteDelay, int nbDoubleNotes)
        {
            difficultyContent.SetActive(true);
            _currentLevelName = levelName;
            numberOfNotesTextTMP.text = nbNotes.ToString();
            var minutes = audioLength / 60;
            var seconds = audioLength % 60;
            audioLengthTextTMP.text = Mathf.Floor(minutes)+" m "+ Mathf.RoundToInt(seconds )+" s";
            numberOfNotesPerSecondTextTMP.text = nbNotesPerSec.ToString("F3");
            minimalNoteDelayTextTMP.text = minNoteDelay.ToString("F2");
            numberOfDoubleNotesTextTMP.text = nbDoubleNotes.ToString();

            string scoreKey = levelName + PlayerPrefsManager.ScoreSuffix;
            string comboKey = levelName + PlayerPrefsManager.ComboSuffix;
            bestScoreTextTMP.text = PlayerPrefsManager.HasPref(scoreKey) ? PlayerPrefsManager.GetPref(scoreKey, 0f).ToString("F0") : "None";
            bestComboTextTMP.text = PlayerPrefsManager.HasPref(comboKey) ? PlayerPrefsManager.GetPref(comboKey, 0f).ToString("F0") : "None";
        }

        public void UnsetDifficulty()
        {
            difficultyContent.SetActive(false);
        }

        public void DeleteLevel()
        {
            if (!string.IsNullOrEmpty(_currentLevelName))
            {
                LevelTools.DeleteLevelFolder(_currentLevelName);
                PlayerPrefsManager.DeletePref(_currentLevelName + PlayerPrefsManager.ScoreSuffix);
                PlayerPrefsManager.DeletePref(_currentLevelName + PlayerPrefsManager.ComboSuffix);
                SetList();
            }
        }

        public void ShareLevel()
        {
            if (!string.IsNullOrEmpty(_currentLevelName))
            {
                Application.OpenURL("file://" + LevelTools.GetAllLevelsFolderPath());
            }
        }
    }
}
