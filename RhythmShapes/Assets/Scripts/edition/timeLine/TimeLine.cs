using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace edition.timeLine
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

        public static float RealWidth => Width + StartOffset * 2;

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

        public void UpdateWidth()
        {
            AudioClip clip = audioSource.clip;
            float audioLen = clip != null && clip.length > 0f ? clip.length : 1f;
            Width = audioLen * WidthPerLength;
            
            _transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, RealWidth);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RealWidth);
            //gridLayoutGroup.cellSize = new Vector2(realWidth, gridLayoutGroup.cellSize.y);
        }
    }
}
