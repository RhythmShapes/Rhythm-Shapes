using System;
using TMPro;
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
        }

        private void OnClickButton()
        {
            GameInfo.LevelName = _levelName;
            GameInfo.IsNewLevel = false;
            SceneTransition.Instance.LoadNextScene();
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
            MusicSelectionManager.Instance.SetLevelDifficultyText(numberOfNotes,audioLength,numberOfNotesPerSecond,minimalNoteDelay,numberOfDoubleNotes);
            //Debug.Log(NumberOfNotes+", "+NumberOfNotesPerSecond+", "+MinimalNoteDelay+", "+AnalysisThreshold+", "+DoubleNoteAnalysisThreshold);
        }
    }
}