using System;
using System.Collections.Generic;
using System.Globalization;
using AudioAnalysis;
using shape;
using UnityEngine;
using UnityEngine.Events;
using utils.XML;

namespace edition
{
    public class ShapeTimeLine : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject squareShapePrefab;
        [SerializeField] private GameObject circleShapePrefab;
        [SerializeField] private GameObject diamondShapePrefab;
        [SerializeField] private Transform topLine;
        [SerializeField] private Transform rightLine;
        [SerializeField] private Transform leftLine;
        [SerializeField] private Transform bottomLine;
        [SerializeField] private Color topColor;
        [SerializeField] private Color rightColor;
        [SerializeField] private Color leftColor;
        [SerializeField] private Color bottomColor;
        [SerializeField] private UnityEvent onDisplayDone;
        [SerializeField] private UnityEvent onShapeSelected;

        private readonly List<EditorShape> _shapes = new();
        private float _posXCorrection = 0f;

        private static ShapeTimeLine _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;
        }

        private void Start()
        {
            _posXCorrection = topLine.position.x;
        }

        public void DisplayLevel(LevelDescription level)
        {
            foreach (var shape in _shapes)
            {
                Destroy(shape.gameObject);
            }
            _shapes.Clear();
            
            if (level.shapes != null)
            {
                foreach (var shape in level.shapes)
                {
                    CreateShape(shape);
                }
            }
            
            EditorModel.Shape = null;
            onDisplayDone.Invoke();
        }

        public static void ForceSelectShape(ShapeDescription selectShape)
        {
            foreach (var editorShape in _instance._shapes)
            {
                ShapeDescription shape = editorShape.Description;
                
                if (selectShape.IsEqualTo(shape))
                {
                    EditorModel.Shape = editorShape;
                    _instance.onShapeSelected.Invoke();
                    return;
                }
            }
        }
        
        public void OnCreateShape(float time)
        {
            OnCreateShape(time, Target.Top);
        }

        public void OnCreateShape(float time, Target target)
        {
            if (!EditorModel.HasLevelSet())
            {
                NotificationsManager.ShowError("A music has to be analysed before creating a new shape.");
                return;
            }
            
            LevelDescription level = EditorModel.GetCurrentLevel();

            if (level.shapes == null)
                return;
            
            /*foreach (var shapeDescription in level.shapes)
            {
                if (shapeDescription.target == target && Mathf.Abs(shapeDescription.timeToPress - time) < MultiRangeAnalysis.minimalNoteDelay)
                {
                    NotificationsManager.ShowError("Cannot create shape at " + time.ToString(CultureInfo.InvariantCulture) + "s, because another shape is to close. Try changing the Minimal delay between notes value.");
                    return;
                }
            }*/

            ShapeDescription shape = new ShapeDescription()
            {
                goRight = true,
                target = target,
                timeToPress = time,
                type = ShapeType.Square
            };

            ShapeDescription[] shapes = new ShapeDescription[level.shapes.Length + 1];
            Array.Copy(level.shapes, shapes, level.shapes.Length);
            level.shapes = shapes;
            level.shapes[^1] = shape;

            EditorModel.HasShapeBeenModified = true;
            DisplayLevel(level);
            ForceSelectShape(shape);
        }

        public static void OnDeleteShapeStatic(EditorShape shape)
        {
            _instance.OnDeleteShape(shape);
        }

        public void OnDeleteShape()
        {
            OnDeleteShape(null);
        }

        public void OnDeleteShape(EditorShape shape)
        {
            shape ??= EditorModel.Shape;

            if (shape != null)
            {
                LevelDescription level = EditorModel.GetCurrentLevel();
                if (level.shapes == null)
                    return;
                
                ShapeDescription[] shapes = new ShapeDescription[level.shapes.Length - 1];
                
                for (int i = 0, j = 0; i < level.shapes.Length; i++, j++)
                {
                    if (level.shapes[i].Equals(shape.Description))
                    {
                        j--;
                        continue;
                    }

                    shapes[j] = level.shapes[i];
                }
                
                level.shapes = shapes;
                EditorModel.HasShapeBeenModified = true;
                FindObjectOfType<PathDemo>().OnReset();

                EditorShape currentSelected = EditorModel.Shape;
                DisplayLevel(level);

                if (currentSelected != null && !currentSelected.IsEqualTo(shape))
                    ForceSelectShape(currentSelected.Description);
                else
                    EditorModel.Shape = null;
                Destroy(shape.gameObject);
            }
        }

        public void UpdateTimeLine()
        {
            foreach (var shape in _shapes)
            {
                shape.UpdatePosX(GetPosX(shape.Description.timeToPress));
            }
        }

        private EditorShape CreateShape(ShapeDescription shape)
        {
            GameObject shapeType = shape.type switch
            {
                ShapeType.Square => squareShapePrefab,
                ShapeType.Circle => circleShapePrefab,
                ShapeType.Diamond => diamondShapePrefab,
                _ => throw new ArgumentOutOfRangeException()
            };
                
            GameObject createdShape = shape.target switch
            {
                Target.Top => Instantiate(shapeType, topLine),
                Target.Right => Instantiate(shapeType, rightLine),
                Target.Left => Instantiate(shapeType, leftLine),
                Target.Bottom => Instantiate(shapeType, bottomLine),
                _ => throw new ArgumentOutOfRangeException()
            };
                
            Color color = shape.target switch
            {
                Target.Top => topColor,
                Target.Right => rightColor,
                Target.Left => leftColor,
                Target.Bottom => bottomColor,
                _ => throw new ArgumentOutOfRangeException()
            };
            EditorShape editorShape = createdShape.GetComponent<EditorShape>();
            editorShape.Init(shape, GetPosX(shape.timeToPress), color, () =>
            {
                EditorModel.Shape = editorShape;
                onShapeSelected.Invoke();
            });

            _shapes.Add(editorShape);
            return editorShape;
        }

        public static float GetPosX(float timeToPress)
        {
            AudioClip clip = _instance.audioSource.clip;
            float audioLen = clip != null && clip.length > 0f ? clip.length : 1f;
            float ratio = timeToPress / audioLen;
            return TimeLine.StartOffset + ratio * TimeLine.Width;
        }

        public static float GetTimeFromPos(float posX)
        {
            AudioClip clip = _instance.audioSource.clip;
            float audioLen = clip != null && clip.length > 0f ? clip.length : 1f;
            float time = posX / TimeLine.Width * audioLen;
            
            return Mathf.Clamp(time, 0f, audioLen);
        }

        private float GetCorrectedStartPos()
        {
            return topLine.position.x - _posXCorrection;
        }

        public static float GetCorrection()
        {
            return _instance._posXCorrection;
        }

        public void UpdateSelectedType(ShapeType type)
        {
            EditorModel.Shape.Description.type = type;
            _shapes.Remove(EditorModel.Shape);
            Destroy(EditorModel.Shape.gameObject);
            EditorModel.Shape = CreateShape(EditorModel.Shape.Description);
        }

        public void UpdateSelectedTarget(Target target)
        {
            EditorModel.Shape.Description.target = target;
            _shapes.Remove(EditorModel.Shape);
            Destroy(EditorModel.Shape.gameObject);
            EditorModel.Shape = CreateShape(EditorModel.Shape.Description);
        }

        public void UpdateSelectedGoRight(bool goRight)
        {
            EditorModel.Shape.Description.goRight = goRight;
        }

        public void UpdateSelectedPressTime(float pressTime)
        {
            EditorModel.Shape.Description.timeToPress = pressTime;
            EditorModel.Shape.UpdatePosX(GetPosX(pressTime));
        }
    }
}