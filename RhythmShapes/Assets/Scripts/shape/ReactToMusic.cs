using UnityEngine;

namespace shape
{
    public class ReactToMusic : MonoBehaviour
    {
        private float _originalScale = 1f;

        private void Start()
        {
            _originalScale = transform.localScale.x;
        }
        
        void Update()
        {
            float scale = ReactToMusicManager.GetScale(_originalScale);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
