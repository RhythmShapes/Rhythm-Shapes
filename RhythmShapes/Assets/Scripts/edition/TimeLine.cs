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

        private float _widthPerLengthScale = 1f;
        private float _width = 0f;
        
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

        public static float Width => _instance._width;

        public static float WidthPerLength => _instance.widthPerLength * WidthPerLengthScale;

        public static float StartOffset => _instance.startOffset * WidthPerLengthScale;
        
        private static TimeLine _instance;
        private RectTransform _transform;

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
            _width = audioSource.clip.length * WidthPerLength;
            float realWidth = StartOffset + _width;
            //_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, realWidth);
            _transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, realWidth);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, realWidth);
            gridLayoutGroup.cellSize = new Vector2(realWidth, gridLayoutGroup.cellSize.y);
        }
    }
}
