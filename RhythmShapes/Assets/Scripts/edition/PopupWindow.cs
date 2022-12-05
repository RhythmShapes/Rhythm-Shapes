using System;
using TMPro;
using UnityEngine;

namespace edition
{
    public class PopupWindow : MonoBehaviour
    {
        [SerializeField] private GameObject hiddenContent;
        [SerializeField] private GameObject errorIcon;
        [SerializeField] private GameObject infoButtons;
        [SerializeField] private GameObject questionButtons;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI title;

        private Action<bool> _questionCallback;
        private Action _infoCallback;

        public void ShowInfo(string message, string titleValue = "Information", Action callback = null)
        {
            _infoCallback = callback;
            text.text = message;
            title.text = titleValue;
            infoButtons.SetActive(true);
            questionButtons.SetActive(false);
            errorIcon.SetActive(false);
            hiddenContent.SetActive(true);
        }

        public void ShowQuestion(string message, string titleValue = "Question", Action<bool> callback = null)
        {
            _questionCallback = callback;
            text.text = message;
            title.text = titleValue;
            infoButtons.SetActive(false);
            questionButtons.SetActive(true);
            errorIcon.SetActive(false);
            hiddenContent.SetActive(true);
        }
        
        public void ShowError(string message, string titleValue = "Error", Action callback = null)
        {
            _infoCallback = callback;
            text.text = message;
            title.text = titleValue;
            infoButtons.SetActive(true);
            questionButtons.SetActive(false);
            errorIcon.SetActive(true);
            hiddenContent.SetActive(true);
        }
        
        public void Hide()
        {
            hiddenContent.SetActive(false);
        }

        public void OnConfirm(bool confirm)
        {
            Hide();
            if (_questionCallback != null)
            {
                _questionCallback.Invoke(confirm);
                _questionCallback = null;
            }
        }

        public void OnOk()
        {
            Hide();
            if (_infoCallback != null)
            {
                _infoCallback.Invoke();
                _infoCallback = null;
            }
        }
    }
}
