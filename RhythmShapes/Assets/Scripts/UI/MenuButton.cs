using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using utils.XML;

namespace ui
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, ISelectHandler
    {
        private string _levelName;
        public int numberOfNotes;
        public float numberOfNotesPerSecond;
        public float minimalNoteDelay;
        public int numberOfDoubleNotes;
        public float audioLength;
        private bool _launchEditor;

        public void Init(string levelName,LevelDescription levelDescription)
        {
            _levelName = levelName;
            numberOfNotes = levelDescription.numberOfNotes;
            numberOfNotesPerSecond = levelDescription.numberOfNotesPerSecond;
            minimalNoteDelay = levelDescription.minimalNoteDelay;
            numberOfDoubleNotes = levelDescription.numberOfDoubleNotes;
            audioLength = levelDescription.audioLength;
            GetComponentInChildren<TextMeshProUGUI>().text = _levelName;
            GetComponent<Button>().onClick.AddListener(OnClickButton);
            _launchEditor = false;
        }
        
        public void Init(string levelName,LevelDescription levelDescription, bool launchEditor)
        {
            Init(levelName, levelDescription);
            _launchEditor = launchEditor;
        }

        private void OnClickButton()
        {
            if (_launchEditor)
            {
                GameInfo.LevelName = _levelName;
                GameInfo.IsNewLevel = false;
                SceneTransition.Instance.LoadScene(2);
            }
            else
            {
                GameInfo.LevelName = _levelName;
                GameInfo.IsNewLevel = false;
                SceneTransition.Instance.LoadNextScene();
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnClickButton();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_launchEditor)
                EditorMusicSelectionManager.Instance.SetLevelDifficultyText(_levelName,numberOfNotes,audioLength,numberOfNotesPerSecond,minimalNoteDelay,numberOfDoubleNotes);
            else
                MusicSelectionManager.Instance.SetLevelDifficultyText(_levelName,numberOfNotes,audioLength,numberOfNotesPerSecond,minimalNoteDelay,numberOfDoubleNotes);
        }
    }
}