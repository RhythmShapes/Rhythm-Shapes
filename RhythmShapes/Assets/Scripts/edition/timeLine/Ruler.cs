using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace edition.timeLine
{
    public class Ruler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioPlayer audioPlayer;
        [SerializeField] private Transform graduationsContent;
        [SerializeField] private GameObject graduationPrefab;
        [SerializeField] private GameObject specialGraduationPrefab;
        [SerializeField] private int bonusGraduation = 100;
        [SerializeField] private List<Vector2> graduationThresholds;
        
        private RectTransform _transform;
        private readonly List<Graduation> _graduations = new();
        private Graduation _endGraduation;
        private int _listI = 0;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            Invoke("OnZoom", .05f);
        }

        public void OnZoom()
        {
            float realWidth = TimeLine.StartOffset + TimeLine.Width;
            
            _transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, realWidth);
            //_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, realWidth);

            int listLen = _graduations.Count;
            float precision = GetGraduationPrecision();
            float audioLen = audioPlayer.clip != null ? audioPlayer.clip.length : 0f;

            if (audioLen > 0f)
            {
                _listI = 0;
                for (float i = 0; i < audioLen; i += precision)
                {
                    var graduation = CreateGraduation(listLen);
                    graduation.Init(ShapeTimeLine.GetPosX(i), i.ToString(CultureInfo.InvariantCulture), Color.black);
                }

                if (_endGraduation == null)
                    _endGraduation = Instantiate(specialGraduationPrefab, graduationsContent).GetComponent<Graduation>();
                
                _endGraduation.Init(ShapeTimeLine.GetPosX(audioLen), ((float) Math.Round(audioLen, 1)).ToString(CultureInfo.InvariantCulture), Color.red);

                for (int i = _listI; i < listLen; i++)
                {
                    _graduations[i].gameObject.SetActive(false);
                }
            }
        }

        private Graduation CreateGraduation(int listLen)
        {
            Graduation graduation;

            if (listLen > _listI)
            {
                graduation = _graduations[_listI];
                graduation.gameObject.SetActive(true);
                _listI++;
                return graduation;
            }
            
            graduation = Instantiate(graduationPrefab, graduationsContent).GetComponent<Graduation>();
            _graduations.Add(graduation);
            return graduation;
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
    
        private string TimeToStr(float time)
        {
            int hour = (int) (time / 3600f);
            float rest = time % 3600f;
            int minute = (int) (rest / 60f);
            rest = time % 60f;

            StringBuilder str = new StringBuilder();
            if (hour > 0)
            {
                str.Append(hour).Append(":");
                str.Append(minute.ToString("d2"));
                str.Append(rest.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').Replace(".", ","));
            }
            else if(minute > 0)
            {
                str.Append(minute).Append(":");
                str.Append(rest.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').Replace(".", ","));
            }
            else
            {
                str.Append(rest.ToString(CultureInfo.InvariantCulture).Replace(".", ","));
            }

            return str.ToString();
        }
    }
}
