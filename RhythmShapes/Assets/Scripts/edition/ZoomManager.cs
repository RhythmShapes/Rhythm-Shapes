using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edition
{
    [RequireComponent(typeof(ZoomVariables))]
    public class ZoomManager : ScrollRect
    {
        private InputSystem _input;
        private ZoomVariables _variables;
        private float _scale = 1f;

        protected override void OnEnable()
        {
            base.OnEnable();
            _input = new InputSystem();
            _input.Enable();
            _variables = GetComponent<ZoomVariables>();
        }

        private void Zoom(float scrollValue, Vector2 mousePos)
        {
            float zoom = Mathf.Abs(scrollValue) / scrollValue * -1 * _variables.zoomForce;
            float zoomScale = _scale - zoom;
            //float zoomScale = content.localScale.x - zoom;

            if (zoomScale >= _variables.minZoom && zoomScale <= _variables.maxZoom)
            {
                //content.localScale = new Vector3(zoomScale, _variables.scaleY ? zoomScale : 1, 1);
                _scale = zoomScale;
                TimeLine.WidthPerLengthScale -= zoom;
            }
        }

        public override void OnScroll(PointerEventData eventData)
        {
            float scrollValue = _input.UI.ScrollWheel.ReadValue<float>();
            
            if(scrollValue != 0)
                Zoom(scrollValue, eventData.position);
        }
    }
}