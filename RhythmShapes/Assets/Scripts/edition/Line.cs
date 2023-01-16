using System;
using shape;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edition
{
    public class Line : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private float doubleClickDelay = .5f;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Target target;

        private RectTransform _transform;
        private short _clickCount = 0;
        private float _clickTime = 0;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;
            
            if (_clickCount == 1 && eventData.clickTime - _clickTime <= doubleClickDelay)
            {
                _clickCount = 0;

                float posInScreen = (eventData.position.x - ShapeTimeLine.GetCorrection()) / (Screen.width - ShapeTimeLine.GetCorrection());
                float posInComponent = posInScreen * _transform.rect.width;
                float widthOffset = TimeLine.RealWidth - _transform.rect.width;
                float widthPassed = scrollbar.value * widthOffset;
                float posXInTotalWidth = posInComponent + widthPassed - TimeLine.StartOffset;
                
                InspectorPanel.OnCreateShape(ShapeTimeLine.GetTimeFromPos(posXInTotalWidth), target);
                
                /*Debug.Log("posInScreen : "+ posInScreen +" = ("+ eventData.position.x +" - "+ ShapeTimeLine.GetCorrection()+") / ("+(Screen.width - ShapeTimeLine.GetCorrection())+")");
                Debug.Log("posInComponent : "+ posInComponent +" = "+ posInScreen+" * "+_transform.rect.width);
                Debug.Log("widthOffset : "+ widthOffset +" = "+ TimeLine.RealWidth +" - "+ _transform.rect.width);
                Debug.Log("widthPassed : "+ widthPassed +" = "+ scrollbar.value +" * "+ widthOffset);
                Debug.Log("posXInTotalWidth : "+ posXInTotalWidth +" = "+posInComponent +" + "+ widthPassed);
                Debug.Log("time : "+ time +" = "+ posXInTotalWidth +" / "+ TimeLine.Width +" * "+ 216.99f);*/
            } else
            {
                _clickCount = 1;
                _clickTime = eventData.clickTime;
            }
        }
    }
}
