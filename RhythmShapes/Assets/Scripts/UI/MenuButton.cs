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
        public int NumberOfNotes  { get; private set; }
        public float NumberOfNotesPerSecond  { get; private set; }
        public float MinimalNoteDelay { get; private set; }
        public float AnalysisThreshold { get; private set; }
        public float DoubleNoteAnalysisThreshold { get; private set; }

        public void Init(string levelName,LevelDescription levelDescription)
        {
            _levelName = levelName;
            NumberOfNotes = levelDescription.numberOfNotes;
            NumberOfNotesPerSecond = levelDescription.numberOfNotesPerSecond;
            MinimalNoteDelay = levelDescription.minimalNoteDelay;
            AnalysisThreshold = levelDescription.analysisThreshold;
            DoubleNoteAnalysisThreshold = levelDescription.doubleNoteAnalysisThreshold;
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
            MusicSelectionManager.Instance.SetLevelDifficultyText(NumberOfNotes,NumberOfNotesPerSecond,MinimalNoteDelay,AnalysisThreshold,DoubleNoteAnalysisThreshold);
            //Debug.Log(NumberOfNotes+", "+NumberOfNotesPerSecond+", "+MinimalNoteDelay+", "+AnalysisThreshold+", "+DoubleNoteAnalysisThreshold);
        }
    }
}