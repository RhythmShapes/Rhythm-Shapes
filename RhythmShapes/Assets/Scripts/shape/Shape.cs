using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using utils.XML;

namespace shape
{
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class Shape : MonoBehaviour
    {
        public ShapeType Type => _model.Type;
        public Target Target => _model.Target;
        public float TimeToPress => _model.TimeToPress;

        private SpriteRenderer _spriteRenderer;
        private ShapeModel _model;
        private float _maxDistanceDelta = 0;
        private int _currentStep = 0;
        
        public void Init(ShapeModel model)
        {
            _model = model;
            _spriteRenderer.color = model.Color;
            _maxDistanceDelta = 0;
            _currentStep = 0;
            transform.position = _model.PathToFollow[_currentStep];
        }
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Update()
        {
            Vector2 currentPosition = _model.PathToFollow[_currentStep];
            Vector2 targetPosition = _model.PathToFollow[_currentStep + 1];
            
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _maxDistanceDelta);

            if (targetPosition.Equals(transform.position) && _currentStep < _model.PathToFollow.Length - 2)
            {
                _maxDistanceDelta = 0;
                _currentStep++;
            }
            
            _maxDistanceDelta += Time.deltaTime * _model.Speed;
        }
    }
}