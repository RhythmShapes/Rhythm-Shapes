using System;
using shape;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using utils.XML;

namespace edition
{
    public class ShapeTimeLine : MonoBehaviour
    {
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

        public void DisplayLevel(LevelDescription level)
        {
            foreach (var shape in level.shapes)
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
                    Target.Bottom => Instantiate(shapeType, rightLine),
                    Target.Right => Instantiate(shapeType, leftLine),
                    Target.Left => Instantiate(shapeType, bottomLine),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                Color color = shape.target switch
                {
                    Target.Top => topColor,
                    Target.Bottom => rightColor,
                    Target.Right => leftColor,
                    Target.Left => bottomColor,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                EditorShape editorShape = createdShape.GetComponent<EditorShape>();
                editorShape.Init(shape, shape.timeToPress * TimeLine.WidthPerLength, color);
                createdShape.GetComponent<EventTrigger>().triggers[0].callback.AddListener(_ =>
                {
                    EditorModel.Shape = editorShape;
                    onShapeSelected.Invoke();
                }); 
            }
            
            onDisplayDone.Invoke();
        }
    }
}