using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using utils.XML;

namespace edition
{
    [RequireComponent(typeof(Image))]
    public class EditorShape : MonoBehaviour, IPointerClickHandler
    {
        public ShapeDescription Description { get; private set; }
        
        private Image _image;
        private float _startPosX;
        private UnityAction _onClickCallback;
        
        public void Init(ShapeDescription description, float posX, Color color, UnityAction onClick)
        {
            Description = description;
            _onClickCallback = onClick;
            UpdateColor(color);
            UpdatePosX(posX);
        }

        public void UpdatePosX(float posX)
        {
            transform.position = new Vector3(_startPosX + posX, transform.position.y);
        }

        public void UpdateColor(Color color)
        {
            _image.color = color;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _startPosX = transform.position.x;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClickCallback();
        }
    }
}