using shape;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edition
{
    public class Line : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private float doubleClickDelay = .5f;
        [SerializeField] private Target target;
        [SerializeField] private UnityEvent<float, Target> onDoubleClick;

        private RectTransform _transform;
        private short _clickCount = 0;
        private float _clickTime = 0;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            onDoubleClick ??= new UnityEvent<float, Target>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            float time = PosToTime(eventData, scrollbar, _transform.rect.width);
            
            if (_clickCount == 1 && eventData.clickTime - _clickTime <= doubleClickDelay)
            {
                _clickCount = 0;
                onDoubleClick.Invoke(time, target);
            } else
            {
                _clickCount = 1;
                _clickTime = eventData.clickTime;
            }
        }

        public static float PosToTime(PointerEventData eventData, Scrollbar scrollbar, float componentWidth)
        {
            return ShapeTimeLine.GetTimeFromPos(GetPositionInTotalWidth(eventData, scrollbar, componentWidth));
        }

        public static float GetPositionInTotalWidth(PointerEventData eventData, Scrollbar scrollbar, float componentWidth)
        {
            float posInScreen = (eventData.position.x - ShapeTimeLine.GetCorrection()) / (Screen.width - ShapeTimeLine.GetCorrection());
            float posInComponent = posInScreen * componentWidth;
            float widthOffset = TimeLine.RealWidth - componentWidth;
            float widthPassed = scrollbar.value * widthOffset;
            float posXInTotalWidth = posInComponent + widthPassed - TimeLine.StartOffset;
                
            /*Debug.Log("posInScreen : "+ posInScreen +" = ("+ eventData.position.x +" - "+ ShapeTimeLine.GetCorrection()+") / ("+(Screen.width - ShapeTimeLine.GetCorrection())+")");
            Debug.Log("posInComponent : "+ posInComponent +" = "+ posInScreen+" * "+_transform.rect.width);
            Debug.Log("widthOffset : "+ widthOffset +" = "+ TimeLine.RealWidth +" - "+ _transform.rect.width);
            Debug.Log("widthPassed : "+ widthPassed +" = "+ scrollbar.value +" * "+ widthOffset);
            Debug.Log("posXInTotalWidth : "+ posXInTotalWidth +" = "+posInComponent +" + "+ widthPassed);
            Debug.Log("time : "+ time +" = "+ posXInTotalWidth +" / "+ TimeLine.Width +" * "+ 216.99f);*/

            return posXInTotalWidth;
        }
    }
}
