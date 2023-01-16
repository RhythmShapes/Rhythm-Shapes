using System;
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

        private static NotificationsManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;
        }

        public static void ShowInfo(string message)
        {
            AddNotification(message, _instance.infoColor, _instance.infoIcon);
        }

        public static void ShowError(string message)
        {
            AddNotification(message, _instance.errorColor, _instance.errorIcon);
        }

        private static void AddNotification(string message, Color color, Sprite icon)
        {
            Notification notification = Instantiate(_instance.notificationPrefab).GetComponent<Notification>();
            notification.Init(message, color, _instance.lifeTime, icon);
            notification.gameObject.transform.SetParent(_instance.parentComponent, false);
        }
    }
}