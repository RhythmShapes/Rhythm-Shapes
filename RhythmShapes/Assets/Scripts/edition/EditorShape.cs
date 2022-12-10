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
        
        public void Init(ShapeDescription description, float posX, Color color)
        {
            Description = description;
            _image.color = color;
            transform.position = new Vector3(transform.position.x + posX, transform.position.y);
        }
    
        private void Awake()
        {
            _image = GetComponent<Image>();
        }
    }
}