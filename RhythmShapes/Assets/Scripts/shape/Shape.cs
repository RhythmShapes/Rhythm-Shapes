using System;
using System.Collections.Generic;
using System.ComponentModel;
using models;
using UnityEngine;
using utils.XML;

namespace shape
{
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class Shape : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer outline;
        
        public ShapeType Type => _model.Type;
        public Target Target => _model.Target;
        public float TimeToPress => _model.TimeToPress;

        private ShapeModel _model;
        private float _maxDistanceDelta = 0;
        private int _currentStep = 0;
        private float _currentTime = 0;
        private float _currentNormalizedTime = 0;
        private float _currentMaxDistanceDelta = 0;
        private float _pathLength;
        private float[] _pathDistances;
        
        public void Init(ShapeModel model)
        {
            _model = model;
            background.color = model.Color;
            outline.color = model.Color;
            background.sortingOrder = 2;
            outline.sortingOrder = 1;
            _maxDistanceDelta = 0;
            _currentStep = 0;
            transform.position = _model.PathToFollow[0];
            _currentNormalizedTime = 0;
            _currentTime = 0;
            _pathDistances = new float[_model.PathToFollow.Length];
            _pathDistances[0] = 0f;
            _pathLength = 0;
            _currentMaxDistanceDelta = 0;
            for (int i = 1; i < _model.PathToFollow.Length; i++)
            {
                _pathLength += Vector2.Distance(_model.PathToFollow[i], _model.PathToFollow[i - 1]);
                _pathDistances[i] = _pathLength;
            }
        }
    
        public void ShowOutline()
        {
            outline.color = Color.white;
            background.sortingOrder = 2;
            outline.sortingOrder = 1;
        }

        public void ShiftTimeToPress(float amount)
        {
            _model.TimeToPress -= amount;
        }

        void Update()
        {
            Vector2 currentPosition = _model.PathToFollow[_currentStep];
            Vector2 targetPosition = _model.PathToFollow[_currentStep + 1];

            //transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _maxDistanceDelta);
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _currentMaxDistanceDelta);
            /*SKETCH
             transform.position = Vector3.MoveTowards(currentPosition, targetPosition, pathLength*transition(currentNormalizedTime)-_pathDistances[currentStep]);
             */
            if (transform.position.Equals(targetPosition) && _currentStep < _model.PathToFollow.Length - 2)
            {
                _maxDistanceDelta = 0;
                _currentStep++;
            }

            if (!GameModel.Instance.isGamePaused)
            {
                _maxDistanceDelta += Time.deltaTime * _model.Speed;
                _currentTime += Time.deltaTime;
                _currentNormalizedTime = _currentTime / LevelPreparator.TravelTime;
                _currentMaxDistanceDelta = _pathLength * Mathf.Clamp01(TransitionCurves.CubicBezier(0f, .3f, 1f, .7f, _currentNormalizedTime).y) - _pathDistances[_currentStep];
                
            }



        }
    }
}