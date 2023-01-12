using UnityEngine;

namespace edition
{
    public class EndLine : MonoBehaviour
    {
        private float _startPosX;
        
        public void Init(float posX)
        {
            UpdatePosX(posX);
        }

        public void UpdatePosX(float posX)
        {
            transform.position = new Vector3(_startPosX + posX, transform.position.y);
        }

        private void Awake()
        {
            _startPosX = transform.position.x;
        }
    }
}