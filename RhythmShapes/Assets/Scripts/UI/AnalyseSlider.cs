using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
    public class AnalyseSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void Start()
        {
            StartCoroutine(UpdateCo());
        }

        public void Stop()
        {
            StopCoroutine(UpdateCo());
        }

        private IEnumerator UpdateCo()
        {
            slider.value = 0;
            
            while (Math.Abs(slider.value - slider.maxValue) >= 0)
            {
                slider.value += 0.001f;
                yield return null;
            }
            
            yield return null;
        }
    }
}
