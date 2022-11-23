using System;
using System.Collections;
using System.Threading;
using AudioAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
    public class AnalyseSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private Thread _thread;
        private Coroutine _coroutine = null;

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
            slider.value = 0;
            
            while (MultiRangeAnalysis.Progress.IsComplete())
            {
                slider.value = slider.maxValue * MultiRangeAnalysis.Progress.ToPercent();
                yield return null;
            }
            
            yield return null;
        }
    }
}
