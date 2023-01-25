
using edition.test;
using UnityEngine;

namespace edition.timeLine
{
    public class NoSpawnZone : TestLine
    {
        private RectTransform _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void Init(float posX, float width)
        {
            UpdateSize(width);
            UpdatePosX(posX + width / 2);
        }

        public void UpdateSize(float size)
        {
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }
    }
}