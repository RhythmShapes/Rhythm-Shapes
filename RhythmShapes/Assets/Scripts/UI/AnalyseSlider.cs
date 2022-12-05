using System.Collections;
using AudioAnalysis;
using UnityEngine;
using UnityEngine.UI;
using utils;

namespace ui
{
    public class AnalyseSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private Coroutine _coroutine;

        public void Start()
        {
            Stop();
            _coroutine = StartCoroutine(UpdateCo());
        }

        public void Stop()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private IEnumerator UpdateCo()
        {
            ProgressUtil.Reset();
            slider.value = 0;
            
            while (ProgressUtil.IsComplete())
            {
                slider.value = slider.maxValue * ProgressUtil.ToPercent();
                yield return null;
            }
            
            yield return null;
        }
    }
}
