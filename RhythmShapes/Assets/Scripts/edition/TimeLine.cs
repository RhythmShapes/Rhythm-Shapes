using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace edition
{
    [RequireComponent(typeof(RectTransform))]
    public class TimeLine : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private float widthPerLength = 1f;
        [SerializeField] private float startOffset = 10f;
        [SerializeField] private UnityEvent onWidthPerLengthChanged;
        [SerializeField] private RectTransform content;
        
        public static float WidthPerLengthScale
        {
            get => _instance._widthPerLengthScale;
            set
            {
                _instance._widthPerLengthScale = value;
                _instance.UpdateWidth();
                _instance.onWidthPerLengthChanged.Invoke();
            }
        }

        public static float Width { get; private set; }

        public static float WidthPerLength => _instance.widthPerLength * WidthPerLengthScale;

        public static float StartOffset => _instance.startOffset;
        
        private static TimeLine _instance;
        private RectTransform _transform;
        private float _widthPerLengthScale = 1f;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;

            _transform = GetComponent<RectTransform>();
            onWidthPerLengthChanged ??= new UnityEvent();
            
            UpdateWidth();
        }

        private void UpdateWidth()
        {
            AudioClip clip = audioSource.clip;
            float audioLen = clip != null && clip.length > 0f ? clip.length : 1f;
            Width = audioLen * WidthPerLength;
            float realWidth = StartOffset + Width;
            
            _transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, realWidth);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, realWidth);
            //gridLayoutGroup.cellSize = new Vector2(realWidth, gridLayoutGroup.cellSize.y);
        }
    }
}
