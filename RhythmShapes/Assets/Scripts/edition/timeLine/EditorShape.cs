using System;
using edition.test;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using utils.XML;

namespace edition.timeLine
{
    [RequireComponent(typeof(Image))]
    public class EditorShape : TestLine, IPointerClickHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image outline;
        [SerializeField] private float doubleClickDelay = .5f;
        public ShapeDescription Description { get; private set; }

        private Animator _animator;
        private UnityAction _onClickCallback;
        private short _clickCount = 0;
        private float _clickTime = 0;
        private bool _showOutline;
        private static readonly int IsSelected = Animator.StringToHash("IsSelected");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Init(ShapeDescription description, float posX, Color color, UnityAction onClick, UnityAction onDragStart)
        {
            Description = description;
            _onClickCallback = onClick;
            GetComponent<DraggableShape>().SetDragCallbacks(onDragStart);
            UpdateColor(color);
            UpdatePosX(posX);
        }

        public void UpdateColor(Color color)
        {
            background.color = color;
            outline.color = _showOutline ? Color.white : color;
        }

        public void ShowOutline(bool show)
        {
            _showOutline = show;
            UpdateColor(background.color);
        }

        public void ForceInvokeClickCallback()
        {
            _onClickCallback.Invoke();
        }

        public void SetBefore(bool before)
        {
            if (!before)
            {
                _animator.SetBool(IsSelected, false);
                return;
            }

            Transform parent = transform.parent;
            transform.SetParent(SceneManager.GetActiveScene().GetRootGameObjects()[0].transform);
            transform.SetParent(parent);
            _animator.SetBool(IsSelected, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_clickCount == 1 && eventData.clickTime - _clickTime <= doubleClickDelay)
                {
                    _clickCount = 0;
                    ShapeTimeLine.OnDeleteShapeStatic(this);
                }
                else
                {
                    _clickCount = 1;
                    _clickTime = eventData.clickTime;
                }
            } else if(eventData.button == PointerEventData.InputButton.Left)
                _onClickCallback();
        }

        public bool IsEqualTo(EditorShape compare)
        {
            return Description.IsEqualTo(compare.Description);
        }
    }
}