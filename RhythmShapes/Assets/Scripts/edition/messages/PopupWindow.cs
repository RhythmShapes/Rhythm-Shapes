using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace edition.messages
{
    public class PopupWindow : MonoBehaviour
    {
        [SerializeField] private GameObject hiddenContent;
        [SerializeField] private GameObject infoButtons;
        [SerializeField] private GameObject questionButtons;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Sprite errorIcon;
        [SerializeField] private Sprite infoIcon;
        [SerializeField] private Sprite questionIcon;

        private Action<bool> _questionCallback;
        private Action _infoCallback;

        public void ShowInfo(string message, string titleValue = "Information", Action callback = null)
        {
            _infoCallback = callback;
            Show(message, titleValue, infoIcon, false);
        }

        public void ShowQuestion(string message, string titleValue = "Question", Action<bool> callback = null)
        {
            _questionCallback = callback;
            Show(message, titleValue, questionIcon, true);
        }
        
        public void ShowError(string message, string titleValue = "Error", Action callback = null)
        {
            _infoCallback = callback;
            Show(message, titleValue, errorIcon, false);
        }

        private void Show(string message, string titleValue, Sprite iconImage, bool isQuestion)
        {
            text.text = message;
            title.text = titleValue;
            infoButtons.SetActive(!isQuestion);
            questionButtons.SetActive(isQuestion);
            icon.sprite = iconImage;
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
