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
        [SerializeField] private TMP_Dropdown goRightField;
        [SerializeField] private UnityEvent<ShapeType> onRequestChangeType;
        [SerializeField] private UnityEvent<Target> onRequestChangeTarget;
        [SerializeField] private UnityEvent<bool> onRequestChangeGoRight;
        [SerializeField] private UnityEvent<float> onRequestChangeTimeToPress;

        private void Awake()
        {
            onRequestChangeType ??= new UnityEvent<ShapeType>();
            onRequestChangeTarget ??= new UnityEvent<Target>();
            onRequestChangeGoRight ??= new UnityEvent<bool>();
            onRequestChangeTimeToPress ??= new UnityEvent<float>();
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                if (!EditorModel.IsInspectingShape())
                {
                    emptyContentPanel.SetActive(true);
                    contentPanel.SetActive(false);
                }
                else
                {
                    typeField.SetValueWithoutNotify((int) EditorModel.Shape.Description.type);
                    targetField.SetValueWithoutNotify((int) EditorModel.Shape.Description.target);
                    pressTimeField.SetTextWithoutNotify(EditorModel.Shape.Description.timeToPress.ToString(CultureInfo.InvariantCulture));
                    goRightField.SetValueWithoutNotify(EditorModel.Shape.Description.goRight ? 1 : 0);
                    
                    emptyContentPanel.SetActive(false);
                    contentPanel.SetActive(true);
                }
            }

            gameObject.SetActive(active);
        }

        public void OnChangeType(Int32 type)
        {
            onRequestChangeType.Invoke((ShapeType) type);
        }

        public void OnChangeTarget(Int32 target)
        {
            onRequestChangeTarget.Invoke((Target) target);
        }

        public void OnChangeGoRight(Int32 goRight)
        {
            // 0 = left, 1 = right
            onRequestChangeGoRight.Invoke(goRight == 1);
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
