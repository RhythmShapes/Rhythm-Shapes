using edition.test;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace edition.timeLine
{
    public class DraggableShape : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _transform;
        private CanvasGroup _canvasGroup;
        private UnityAction _onDragBeginCallback;
        private Vector3 _originPosition;
        private bool _isDragging;
        
        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetDragCallbacks(UnityAction onDragBegin)
        {
            _onDragBeginCallback = onDragBegin;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            _originPosition = _transform.anchoredPosition;
            _isDragging = !TestManager.IsTestRunning;
            _onDragBeginCallback.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!TestManager.IsTestRunning && _isDragging)
                _transform.anchoredPosition += eventData.delta / ShapeTimeLine.CanvasScaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            _transform.anchoredPosition = _originPosition;
            _isDragging = false;
        }
    }
}