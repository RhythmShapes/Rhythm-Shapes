using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace edition.messages
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textZone;
        [SerializeField] private Image background;
        [SerializeField] private Image icon;

        private float _deathTime;

        public void Init(string message, Color color, float lifeTime, Sprite iconImage)
        {
            textZone.text = message;
            background.color = color;
            _deathTime = Time.time + lifeTime;
            icon.sprite = iconImage;
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