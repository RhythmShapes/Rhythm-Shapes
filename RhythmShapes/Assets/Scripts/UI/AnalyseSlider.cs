using System.Collections;
using AudioAnalysis;
using UnityEngine;
using UnityEngine.UI;
using utils;

namespace ui
{
    public class AnalyseSlider : MonoBehaviour
    {
        public static ProgressUtil Progress { get; } = new();
        
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
            Progress.Reset();
            slider.value = 0;
            
            while (Progress.IsComplete())
            {
                slider.value = slider.maxValue * Progress.ToPercent();
                yield return null;
            }
            
            yield return null;
        }
    }
}
