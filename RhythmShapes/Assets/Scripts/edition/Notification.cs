using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace edition
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textZone;
        [SerializeField] private Image background;

        private float _deathTime;

        public void Init(string message, Color color, float lifeTime)
        {
            textZone.text = message;
            background.color = color;
            _deathTime = Time.time + lifeTime;
        }

        private void FixedUpdate()
        {
            if (Time.time >= _deathTime)
                OnClose();
        }

        public void OnClose()
        {
            Destroy(gameObject);
        }
    }
}