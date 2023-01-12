using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace edition
{
    public class Ruler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform graduationsContent;
        [SerializeField] private GameObject graduationPrefab;
        [SerializeField] private int bonusGraduation = 100;
        [SerializeField] private List<Vector2> graduationThresholds;
        
        private RectTransform _transform;
        private readonly List<Graduation> _graduations = new();

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void OnZoom()
        {
            float realWidth = TimeLine.StartOffset + TimeLine.Width;
            
            _transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, realWidth);
            //_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, realWidth);

            int listLen = _graduations.Count;
            float precision = GetGraduationPrecision();
            float total = audioSource.clip.length + bonusGraduation;

            int listI = 0;
            for (float i = 0; i < total; i += precision)
            {
                Graduation graduation;

                if (listLen > listI)
                {
                    graduation = _graduations[listI];
                    graduation.gameObject.SetActive(true);
                    listI++;
                }
                else
                {
                    graduation = Instantiate(graduationPrefab, graduationsContent).GetComponent<Graduation>();
                    _graduations.Add(graduation);
                }

                graduation.Init(ShapeTimeLine.GetPosX(i), Math.Round(i, 1).ToString(CultureInfo.InvariantCulture));
            }

            for (int i = listI; i < listLen; i++)
            {
                _graduations[i].gameObject.SetActive(false);
            }
        }

        private float GetGraduationPrecision()
        {
            foreach (var threshold in graduationThresholds)
            {
                if (TimeLine.WidthPerLength > threshold.x)
                    return threshold.y;
            }

            return graduationThresholds[^1].y;
        }
    }
}
