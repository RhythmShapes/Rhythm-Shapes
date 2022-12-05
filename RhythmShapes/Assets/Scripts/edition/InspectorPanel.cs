using System;
using System.Globalization;
using shape;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using utils.XML;

namespace edition
{
    public class InspectorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject emptyContentPanel;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private TMP_Dropdown typeField;
        [SerializeField] private TMP_Dropdown targetField;
        [SerializeField] private TMP_InputField pressTimeField;
        [SerializeField] private Toggle goRightField;
        [SerializeField] private UnityEvent<ShapeType> onRequestChangeType;
        [SerializeField] private UnityEvent<Target> onRequestChangeTarget;
        [SerializeField] private UnityEvent<bool> onRequestChangeGoRight;
        [SerializeField] private UnityEvent<float> onRequestChangeTimeToPress;
        
        public static InspectorPanel Instance { get; private set; }
        
        private ShapeDescription _shape;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            onRequestChangeType ??= new UnityEvent<ShapeType>();
            onRequestChangeTarget ??= new UnityEvent<Target>();
            onRequestChangeGoRight ??= new UnityEvent<bool>();
            onRequestChangeTimeToPress ??= new UnityEvent<float>();
        }

        public void SetShape(ShapeDescription shape)
        {
            _shape = shape;
        }

        public bool IsShapeDefined()
        {
            return _shape != null;
        }

        public ShapeDescription GetShape()
        {
            return _shape;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                if (!IsShapeDefined())
                {
                    emptyContentPanel.SetActive(false);
                    contentPanel.SetActive(true);
                }
                else
                {
                    typeField.SetValueWithoutNotify((int) _shape.type);
                    targetField.SetValueWithoutNotify((int) _shape.target);
                    pressTimeField.SetTextWithoutNotify(_shape.timeToPress.ToString(CultureInfo.InvariantCulture));
                    goRightField.SetIsOnWithoutNotify(_shape.goRight);
                    
                    emptyContentPanel.SetActive(false);
                    contentPanel.SetActive(true);
                }
            }

            gameObject.SetActive(active);
        }

        public void OnChangeType(Int32 type)
        {
            if(IsShapeDefined())
                onRequestChangeType.Invoke((ShapeType) type);
        }

        public void OnChangeTarget(Int32 target)
        {
            if(IsShapeDefined())
                onRequestChangeTarget.Invoke((Target) target);
        }

        public void OnChangeGoRight(bool goRight)
        {
            onRequestChangeGoRight.Invoke(goRight);
        }

        public void OnChangePressTime(string textPressTime)
        {
            if (!float.TryParse(textPressTime.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var pressTime))
            {
                //pressTimeField.GetComponentInParent<ErrorMessage>().ShowError("Invalid float");
                pressTimeField.SetTextWithoutNotify("0");
                return;
            }
            
            pressTime = Mathf.Clamp(pressTime, 0f, audioSource.clip.length);
            pressTimeField.SetTextWithoutNotify(pressTime.ToString(CultureInfo.InvariantCulture));
            onRequestChangeTimeToPress.Invoke(pressTime);
        }
    }
}
