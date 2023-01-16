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
            
            EditorModel.HasShapeBeenModified = true;
            onRequestChangeType.Invoke((ShapeType) type);
        }

        public void OnChangeTarget(int target)
        {
            if((Target) target == EditorModel.Shape.Description.target)
                return;

            EditorModel.HasShapeBeenModified = true;
            onRequestChangeTarget.Invoke((Target) target);
        }

        public void OnChangeGoRight(int goRight)
        {
            if(goRight == 1 && EditorModel.Shape.Description.goRight)
                return;

            EditorModel.HasShapeBeenModified = true;
            // 0 = left, 1 = right
            onRequestChangeGoRight.Invoke(goRight == 1);
        }

        public void OnChangePressTime(string textPressTime)
        {
            if (!float.TryParse(textPressTime.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var pressTime))
            {
                //pressTimeField.GetComponentInParent<ErrorMessage>().ShowError("Invalid float");
                pressTime = 0f;
            }
            
            pressTime = Mathf.Clamp(pressTime, 0f, audioSource.clip.length);
            pressTimeField.SetTextWithoutNotify(pressTime.ToString(CultureInfo.InvariantCulture));

            if(Math.Abs(pressTime - EditorModel.Shape.Description.timeToPress) == 0f)
                return;
            
            EditorModel.HasShapeBeenModified = true;
            onRequestChangeTimeToPress.Invoke(pressTime);
        }

        public void OnCreateShape(float time)
        {
            OnCreateShape(time, Target.Top);
        }

        public static void OnCreateShape(float time, Target target)
        {
            if (!EditorModel.HasLevelSet())
            {
                NotificationsManager.ShowError("A music has to be analysed before creating a new shape.");
                return;
            }
            
            ShapeDescription shape = new ShapeDescription()
            {
                goRight = true,
                target = target,
                timeToPress = time,
                type = ShapeType.Square
            };

            LevelDescription level = EditorModel.HasBeenAnalyzed() ? EditorModel.AnalyzedLevel : EditorModel.OriginLevel;
            ShapeDescription[] shapes = new ShapeDescription[level.shapes.Length + 1];
            Array.Copy(level.shapes, shapes, level.shapes.Length);
            level.shapes = shapes;
            level.shapes[^1] = shape;

            EditorModel.HasShapeBeenModified = true;
            ShapeTimeLine.StaticDisplayLevel(level);
            ShapeTimeLine.ForceSelectShape(shape);
        }

        public void OnDeleteShape()
        {
            OnDeleteShapeStatic(null);
        }

        public static void OnDeleteShapeStatic(EditorShape shape)
        {
            shape ??= EditorModel.Shape;

            if (shape != null)
            {
                LevelDescription level = EditorModel.HasBeenAnalyzed() ? EditorModel.AnalyzedLevel : EditorModel.OriginLevel;
                ShapeDescription[] shapes = new ShapeDescription[level.shapes.Length - 1];
                
                for (int i = 0, j = 0; i < level.shapes.Length; i++, j++)
                {
                    if (level.shapes[i].Equals(shape.Description))
                    {
                        j--;
                        Destroy(shape.gameObject);
                        continue;
                    }

                    shapes[j] = level.shapes[i];
                }
                
                level.shapes = shapes;
                EditorModel.HasShapeBeenModified = true;
                FindObjectOfType<PathDemo>().OnReset();
                ShapeTimeLine.StaticDisplayLevel(level);
            }
        }
    }
}
