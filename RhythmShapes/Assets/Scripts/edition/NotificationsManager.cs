using UnityEngine;

namespace edition
{
    public class NotificationsManager : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform parentComponent;
        [SerializeField] private Color infoColor;
        [SerializeField] private Color errorColor;
        [SerializeField] private Sprite infoIcon;
        [SerializeField] private Sprite errorIcon;
        [SerializeField] [Min(1)] private float lifeTime;

        public void ShowInfo(string message)
        {
            AddNotification(message, infoColor, infoIcon);
        }

        public void ShowError(string message)
        {
            AddNotification(message, errorColor, errorIcon);
        }

        private void AddNotification(string message, Color color, Sprite icon)
        {
            Notification notification = Instantiate(notificationPrefab).GetComponent<Notification>();
            notification.Init(message, color, lifeTime, icon);
            notification.gameObject.transform.SetParent(parentComponent, false);
        }
    }
}