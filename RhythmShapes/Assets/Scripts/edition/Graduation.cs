using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace edition
{
    [RequireComponent(typeof(Image))]
    public class Graduation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        private float _startPosX;
        
        public void Init(float posX, string value, Color color)
        {
            UpdateAll(posX, value);
            text.color = color;
            GetComponent<Image>().color = color;
        }

        public void UpdatePosX(float posX)
        {
            transform.position = new Vector3(_startPosX + posX, transform.position.y);
        }

        public void UpdateAll(float posX, string value)
        {
            UpdatePosX(posX);
            text.SetText(value);
        }

        private void Awake()
        {
            _startPosX = transform.position.x;
        }
    }
}