﻿using System;
using System.Collections.Generic;
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
        //[SerializeField] private EndLine endLine;
        [SerializeField] private Color topColor;
        [SerializeField] private Color rightColor;
        [SerializeField] private Color leftColor;
        [SerializeField] private Color bottomColor;
        [SerializeField] private UnityEvent onDisplayDone;
        [SerializeField] private UnityEvent onShapeSelected;

        private List<EditorShape> _shapes = new();
        private float _posXCorrection = 0f;

        private static ShapeTimeLine _instance;

        private void Start()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;
            
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
                    CreateShape(shape);
            }

            //endLine.Init(GetPosX(audioSource.clip.length));

            onDisplayDone.Invoke();
        }

        public void UpdateTimeLine()
        {
            foreach (var shape in _shapes)
            {
                shape.UpdatePosX(GetPosX(shape.Description.timeToPress));
            }
            
            //endLine.UpdatePosX(GetPosX(audioSource.clip.length));
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
            editorShape.Init(shape, GetPosX(shape.timeToPress), color, () => {
                EditorModel.Shape = editorShape;
                onShapeSelected.Invoke();
            });

            _shapes.Add(editorShape);
            return editorShape;
        }

        public static float GetPosX(float timeToPress)
        {
            float ratio = timeToPress / _instance.audioSource.clip.length;
            return GetCorrection() + TimeLine.StartOffset + ratio * TimeLine.Width;
        }

        public static float GetCorrection()
        {
            return _instance.topLine.position.x - _instance._posXCorrection;
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