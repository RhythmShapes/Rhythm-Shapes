using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edition
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private RectTransform widthRef;
        [SerializeField] private UnityEvent<float> onClick;

        private void Start()
        {
            onClick ??= new UnityEvent<float>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            onClick.Invoke(Line.PosToTime(eventData, scrollbar, widthRef.rect.width));
        }
    }
}