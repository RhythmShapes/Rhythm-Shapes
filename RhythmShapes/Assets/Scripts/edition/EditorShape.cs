using UnityEngine;
using UnityEngine.UI;
using utils.XML;

namespace edition
{
    [RequireComponent(typeof(Image))]
    public class EditorShape : MonoBehaviour
    {
        public ShapeDescription Description { get; private set; }
        
        private Image _image;
        private float _startPosX;
        
        public void Init(ShapeDescription description, float posX, Color color)
        {
            Description = description;
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
    }
}