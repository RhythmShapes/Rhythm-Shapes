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
            Debug.Log("6");
        }

        public void Stop()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private IEnumerator UpdateCo()
        {
            slider.value = 0;
            Debug.Log("5");
            
            while (Math.Abs(slider.value - slider.maxValue) >= 0)
            {
                slider.value += MultiRangeAnalysis.GetProgress();
                yield return null;
            }
            
            yield return null;
        }
    }
}
