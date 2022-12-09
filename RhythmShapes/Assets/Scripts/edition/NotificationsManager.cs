using System.Collections.Generic;
using UnityEngine;

namespace edition
{
    public class NotificationsManager : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform parentComponent;
        [SerializeField] private Color infoColor;
        [SerializeField] private Color errorColor;
        [SerializeField] [Min(1)] private int maxCount;
        [SerializeField] [Min(1)] private float lifeTime;

        private List<Notification> _notifications;

        public void Start()
        {
            _notifications = new List<Notification>(maxCount);
        }

        public void AddInfo(string message)
        {
            AddNotification(message, infoColor);
        }

        public void AddError(string message)
        {
            AddNotification(message, errorColor);
        }

        private void AddNotification(string message, Color color)
        {
            Notification notification = Instantiate(notificationPrefab).GetComponent<Notification>();
            notification.Init(message, color, lifeTime);
            notification.gameObject.transform.SetParent(parentComponent);
            _notifications.Add(notification);
        }
    }
}