using UnityEngine;
using UnityEngine.UI;

namespace edition
{
    [RequireComponent(typeof(RectTransform))]
    public class TimeLine : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private float widthPerLength = 1f;

        public static float WidthPerLength => _instance.widthPerLength;
        
        private static TimeLine _instance;
        private RectTransform _transform;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;
        }

        private void Start()
        {
            float width = audioSource.clip.length * widthPerLength;
            
            _transform = GetComponent<RectTransform>();
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            gridLayoutGroup.cellSize = new Vector2(width, gridLayoutGroup.cellSize.y);
        }
    }
}
