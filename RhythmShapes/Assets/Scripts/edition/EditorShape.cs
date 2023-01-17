using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using utils.XML;

namespace edition
{
    [RequireComponent(typeof(Image))]
    public class EditorShape : TestLine, IPointerClickHandler
    {
        [SerializeField] private float doubleClickDelay = .5f;
        public ShapeDescription Description { get; private set; }

        private Image _image;
        private UnityAction _onClickCallback;
        private short _clickCount = 0;
        private float _clickTime = 0;
        
        public void Init(ShapeDescription description, float posX, Color color, UnityAction onClick)
        {
            Description = description;
            _onClickCallback = onClick;
            UpdateColor(color);
            UpdatePosX(posX);
        }

        public void UpdateColor(Color color)
        {
            _image.color = color;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_clickCount == 1 && eventData.clickTime - _clickTime <= doubleClickDelay)
                {
                    _clickCount = 0;
                    ShapeTimeLine.OnDeleteShapeStatic(this);
                }
                else
                {
                    _clickCount = 1;
                    _clickTime = eventData.clickTime;
                }
            } else if(eventData.button == PointerEventData.InputButton.Left)
                _onClickCallback();
        }

        public bool IsEqualTo(EditorShape compare)
        {
            return Description.IsEqualTo(compare.Description);
        }
    }
}