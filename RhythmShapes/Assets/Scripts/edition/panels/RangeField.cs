using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace edition.panels
{
    public class RangeField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField field;
        [SerializeField] private Slider slider;
        [SerializeField] private float min = 0f;
        [SerializeField] private float max = 1f;
        [SerializeField] private float step = .1f;
        [SerializeField] private UnityEvent<float> onValueChanged;

        private int _rounding = 1;

        private void Awake()
        {
            onValueChanged ??= new UnityEvent<float>();
        }

        private void Start()
        {
            slider.SetValueWithoutNotify(min); // Avoid onValueChanged of slider
            
            slider.minValue = 0;
            slider.maxValue = Mathf.RoundToInt(max / step);
            
            string stepStr = step.ToString(CultureInfo.InvariantCulture);
            _rounding = stepStr[(stepStr.IndexOf(".", StringComparison.Ordinal) + 1)..].Length;
        }

        private void ParseAndSetValue(float value, bool notify = true)
        {
            float steppedValue = Mathf.RoundToInt(value / step) * step;
            float v = (float) Math.Round((decimal) Mathf.Clamp(steppedValue, min, max), _rounding, MidpointRounding.ToEven);
            
            slider.SetValueWithoutNotify(Mathf.RoundToInt(v / step));
            field.SetTextWithoutNotify(v.ToString(CultureInfo.InvariantCulture));
            
            if(notify) onValueChanged.Invoke(value);
        }

        public void SetValueWithoutNotify(float value)
        {
            ParseAndSetValue(value, false);
        }

        public void SetValue(float value)
        {
            ParseAndSetValue(value * step);
        }

        public void SetValue(string value)
        {
            if (!float.TryParse(value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                v = min;
            }

            ParseAndSetValue(v);
        }
    }
}
