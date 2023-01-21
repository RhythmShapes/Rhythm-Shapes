using System;
using System.Collections.Generic;
using System.Globalization;
using AudioAnalysis;
using edition.messages;
using edition.test;
using shape;
using UnityEngine;
using UnityEngine.Events;
using utils;
using utils.XML;

namespace edition.timeLine
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
        [SerializeField] private NoSpawnZone noSpawnZone;
        [SerializeField] private Color topColor;
        [SerializeField] private Color rightColor;
        [SerializeField] private Color leftColor;
        [SerializeField] private Color bottomColor;
        [SerializeField] private Color excludeColor;
        [SerializeField] private UnityEvent onDisplayDone;
        [SerializeField] private UnityEvent onShapeSelected;
        [SerializeField] private UnityEvent onShapeDeselected;
        [SerializeField] private UnityEvent<ShapeDescription, bool> onShapeChanged;

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
            onDisplayDone ??= new UnityEvent();
            onShapeSelected ??= new UnityEvent();
            onShapeDeselected ??= new UnityEvent();
            onShapeChanged ??= new UnityEvent<ShapeDescription, bool>();
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
                    shape.timeToPress = Utils.RoundTime(shape.timeToPress);
                    
                    if(!IsShapeTimeValid(shape.timeToPress))
                        continue;
                    
                    EditorShape created = CreateShape(shape);
                    
                    foreach (var editorShape in _shapes)
                    {
                        if(editorShape == created)
                            continue;

                        if (editorShape.Description.timeToPress.Equals(created.Description.timeToPress))
                        {
                            editorShape.ShowOutline(true);
                            created.ShowOutline(true);
                            break;
                        }
                    }
                }
            }
            
            UpdateNoSpawnZone();
            EditorModel.Shape = null;
            onDisplayDone.Invoke();
        }

        private bool IsShapeTimeValid(float timeToPress)
        {
            float timeToSpawn = LevelPreparator.GetShapeTimeToSpawn(timeToPress);
            return timeToPress >= 0f && timeToPress <= audioSource.clip.length 
                                     && timeToSpawn >= 0f && timeToSpawn <= audioSource.clip.length;
        }

        public void UpdateTimeLine()
        {
            foreach (var shape in _shapes)
            {
                shape.UpdatePosX(GetPosX(shape.Description.timeToPress));
            }

            UpdateNoSpawnZone();
        }

        private void UpdateNoSpawnZone()
        {
            float start = GetPosX(0f);
            noSpawnZone.Init(start, GetPosX(LevelPreparator.TravelTime) - start);
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
            EditorShape editorShape = createdShape.GetComponent<EditorShape>();
            editorShape.Init(shape, GetPosX(shape.timeToPress), GetShapeColor(shape.target), () =>
            {
                if(TestManager.IsTestRunning)
                    return;

                if (EditorModel.Shape == editorShape)
                {
                    DeselectShape();
                    return;
                }
                
                EditorModel.Shape = editorShape;
                onShapeSelected.Invoke();
            });

            _shapes.Add(editorShape);
            return editorShape;
        }

        private Color GetShapeColor(Target target)
        {
            return target switch
            {
                Target.Top => topColor,
                Target.Right => rightColor,
                Target.Left => leftColor,
                Target.Bottom => bottomColor,
                _ => throw new ArgumentOutOfRangeException()
            };
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
            UpdateMultipleShapes();
            ExcludeButSelected();
            onShapeChanged.Invoke(EditorModel.Shape.Description, true);
        }

        public void UpdateSelectedTarget(Target target)
        {
            if (HasCloseShape(EditorModel.Shape.Description, target, EditorModel.Shape.Description.timeToPress))
            {
                NotificationsManager.ShowError("Cannot change shape target to " + 
                                               target + " because another shape is to close of the time : " + 
                                               EditorModel.Shape.Description.timeToPress.ToString(CultureInfo.InvariantCulture) + 
                                               "s. Try changing the Minimal delay between notes value.");
                onShapeChanged.Invoke(EditorModel.Shape.Description, false);
                return;
            }
            
            EditorModel.Shape.Description.target = target;
            _shapes.Remove(EditorModel.Shape);
            Destroy(EditorModel.Shape.gameObject);
            EditorModel.Shape = CreateShape(EditorModel.Shape.Description);
            UpdateMultipleShapes();
            ExcludeButSelected();
            onShapeChanged.Invoke(EditorModel.Shape.Description, true);
        }

        public void UpdateSelectedGoRight(bool goRight)
        {
            EditorModel.Shape.Description.goRight = goRight;
            onShapeChanged.Invoke(EditorModel.Shape.Description, true);
        }

        public void UpdateSelectedPressTime(float pressTime)
        {
            if (!IsShapeTimeValid(pressTime))
            {
                onShapeChanged.Invoke(EditorModel.Shape.Description, false);
                return;
            }

            if (HasCloseShape(EditorModel.Shape.Description, EditorModel.Shape.Description.target, pressTime))
            {
                NotificationsManager.ShowError("Cannot change shape press time to " + 
                                               pressTime.ToString(CultureInfo.InvariantCulture) + "s with " + 
                                               EditorModel.Shape.Description.target + 
                                               " target, because another shape is to close. Try changing the Minimal delay between notes value.");
                onShapeChanged.Invoke(EditorModel.Shape.Description, false);
                return;
            }
            
            EditorModel.Shape.Description.timeToPress = pressTime;
            EditorModel.Shape.UpdatePosX(GetPosX(pressTime));
            UpdateMultipleShapes();
            ExcludeButSelected();
            onShapeChanged.Invoke(EditorModel.Shape.Description, true);
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

        public void DeselectShape()
        {
            EditorModel.Shape = null;
            onShapeDeselected.Invoke();
        }
        
        public void OnCreateShape(float time)
        {
            OnCreateShape(time, Target.Top);
        }

        public void OnCreateShape(float time, Target target)
        {
            if (TestManager.IsTestRunning)
            {
                NotificationsManager.ShowError("Action disabled when test is running.");
                return;
            }
            
            if (!EditorModel.HasLevelSet())
            {
                NotificationsManager.ShowError("A music has to be analysed before creating a new shape.");
                return;
            }
            
            LevelDescription level = EditorModel.GetCurrentLevel();

            if (level.shapes == null)
                return;

            time = Mathf.Clamp(time, LevelPreparator.TravelTime, audioSource.clip.length);

            ShapeDescription shape = new ShapeDescription()
            {
                goRight = true,
                target = target,
                timeToPress = time,
                type = ShapeType.Square
            };
            
            foreach (var shapeDescription in level.shapes)
            {
                if(!IsShapeTimeValid(shapeDescription.timeToPress))
                    continue;
                
                if (HasCloseShape(shape, target, time))
                {
                    NotificationsManager.ShowError("Cannot create shape at " + 
                                                   time.ToString(CultureInfo.InvariantCulture) + "s with " + 
                                                   target + 
                                                   " target, because another shape is to close. Try changing the Minimal delay between notes value.");
                    return;
                }
            }

            ShapeDescription[] shapes = new ShapeDescription[level.shapes.Length + 1];
            Array.Copy(level.shapes, shapes, level.shapes.Length);
            level.shapes = shapes;
            level.shapes[^1] = shape;

            EditorModel.HasShapeBeenModified = true;
            DisplayLevel(level);
            ForceSelectShape(shape);
        }

        private bool HasCloseShape(ShapeDescription description, Target target, float time)
        {
            foreach (var shape in _shapes)
            {
                if (description != shape.Description && target == shape.Description.target
                    && Mathf.Abs(shape.Description.timeToPress - time) < MultiRangeAnalysis.minimalNoteDelay)
                {
                    return true;
                }
            }

            return false;
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
            if (TestManager.IsTestRunning)
            {
                NotificationsManager.ShowError("Action disabled when test is running.");
                return;
            }
            
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
                {
                    EditorModel.Shape = null;
                    ResetExcludeColors();
                }

                Destroy(shape.gameObject);
            }
        }

        public void ExcludeButSelected()
        {
            if (!EditorModel.IsInspectingShape())
                return;

            bool isSelectedMultiple = false;
            foreach (var shape in _shapes)
            {
                bool found = shape == EditorModel.Shape;
                bool isMultiple = shape.Description.timeToPress.Equals(EditorModel.Shape.Description.timeToPress);
                
                shape.UpdateColor(found || isMultiple ? GetShapeColor(shape.Description.target) : excludeColor);
                shape.SetBefore(found);
                isSelectedMultiple = isSelectedMultiple || isMultiple && !found;
            }
        }

        public void ResetExcludeColors()
        {
            foreach (var shape in _shapes)
            {
                shape.UpdateColor(GetShapeColor(shape.Description.target));
                shape.SetBefore(false);
            }
        }

        private void UpdateMultipleShapes()
        {
            foreach (var shape in _shapes) 
                shape.ShowOutline(false);
            
            foreach (var shape in _shapes)
            {
                bool show = false;
                
                foreach (var shape2 in _shapes)
                {
                    bool show2 = shape != shape2 && shape.Description.timeToPress.Equals(shape2.Description.timeToPress);
                    show = show || show2;
                    if(show2) shape2.ShowOutline(true);
                }
                
                shape.ShowOutline(show);
            }
        }
    }
}