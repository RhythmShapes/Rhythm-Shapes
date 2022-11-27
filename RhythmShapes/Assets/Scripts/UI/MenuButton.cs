using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ui
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISubmitHandler
    {
        private string _levelName;

        public void Init(string levelName)
        {
            _levelName = levelName;
            GetComponentInChildren<TextMeshProUGUI>().text = _levelName;
            GetComponent<Button>().onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            GameInfo.LevelName = _levelName;
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
    }
}