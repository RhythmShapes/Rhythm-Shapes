using System;
using System.Collections.Generic;
using System.Threading;
using shape;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

        private List<EditorShape> _shapes = new();

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
                    CreateShape(shape);
            }

            onDisplayDone.Invoke();
        }

        public void UpdateTimeLine()
        {
            foreach (var shape in _shapes)
            {
                float ratio = shape.Description.timeToPress / audioSource.clip.length;
                Debug.Log(shape.Description.target+" : "+shape.Description.timeToPress+ "/" +audioSource.clip.length+" --> "+ratio +" * "+ TimeLine.Width+" = "+(ratio * TimeLine.Width));
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
            editorShape.Init(shape, GetPosX(shape.timeToPress), color);
            createdShape.GetComponent<EventTrigger>().triggers[0].callback.AddListener(_ =>
            {
                EditorModel.Shape = editorShape;
                onShapeSelected.Invoke();
            });

            _shapes.Add(editorShape);
            return editorShape;
        }

        private float GetPosX(float timeToPress)
        {
            float ratio = timeToPress / audioSource.clip.length;
            return TimeLine.StartOffset + ratio * TimeLine.Width;
            //return startOffset + timeToPress * TimeLine.WidthPerLength;
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