using System;
using System.Globalization;
using shape;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using utils;
using utils.XML;

namespace edition.panels
{
    public class InspectorPanel : MonoBehaviour
    {
        [Header("Panel components")]
        [Space]
        [SerializeField] private GameObject emptyContentPanel;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private TMP_Dropdown typeField;
        [SerializeField] private TMP_Dropdown targetField;
        [SerializeField] private TMP_InputField pressTimeField;
        [SerializeField] private TMP_Dropdown goRightField;
        [Space]
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

        public void OnChangeType(int type)
        {
            if((ShapeType) type == EditorModel.Shape.Description.type)
                return;
            
            onRequestChangeType.Invoke((ShapeType) type);
        }

        public void OnChangeTarget(int target)
        {
            if((Target) target == EditorModel.Shape.Description.target)
                return;

            onRequestChangeTarget.Invoke((Target) target);
        }

        public void OnChangeGoRight(int goRight)
        {
            if(goRight == 1 && EditorModel.Shape.Description.goRight)
                return;

            // 0 = left, 1 = right
            onRequestChangeGoRight.Invoke(goRight == 1);
        }

        public void OnChangePressTime(string textPressTime)
        {
            if (!float.TryParse(textPressTime.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var pressTime))
                pressTime = 0f;
            
            pressTime = Mathf.Clamp(pressTime, LevelPreparator.TravelTime, audioSource.clip.length);
            pressTime = Utils.RoundTime(pressTime);
            //pressTimeField.SetTextWithoutNotify(pressTime.ToString(CultureInfo.InvariantCulture));

            if(Math.Abs(pressTime - EditorModel.Shape.Description.timeToPress) == 0f)
                return;
            
            onRequestChangeTimeToPress.Invoke(pressTime);
        }

        public void OnSelectedChanged(ShapeDescription shape, bool changed)
        {
            typeField.SetValueWithoutNotify((int) shape.type);
            targetField.SetValueWithoutNotify((int) shape.target);
            pressTimeField.SetTextWithoutNotify(shape.timeToPress.ToString(CultureInfo.InvariantCulture));
            goRightField.SetValueWithoutNotify(shape.goRight ? 1 : 0);
            if(changed) EditorModel.HasShapeBeenModified = true;
        }
    }
}
