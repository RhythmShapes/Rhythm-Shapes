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
            float zoomScaleX = Mathf.Clamp(content.localScale.x - zoom, _variables.minZoom.x, _variables.maxZoom.x);
            float zoomScaleY = Mathf.Clamp(content.localScale.y - zoom, _variables.minZoom.y, _variables.maxZoom.y);
            
            content.localScale = new Vector3(zoomScaleX, _variables.scaleY ? zoomScaleY : 1, 1);
        }

        public override void OnScroll(PointerEventData eventData)
        {
            float scrollValue = _input.UI.ScrollWheel.ReadValue<float>();
            
            if(scrollValue != 0)
                Zoom(scrollValue, eventData.position);
        }
    }
}