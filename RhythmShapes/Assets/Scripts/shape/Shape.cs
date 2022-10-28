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
        [SerializeField] private ShapeType type;
        public ShapeType Type
        {
            get => type;
            private set
            {
                if (!Enum.IsDefined(typeof(ShapeType), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ShapeType));
                Type = value;
            }
        }

        public float TimeToPress { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private Vector2[] _pathToFollow;
        private float _speed;


        private float _t;
        private int _i;
        private Vector3 _startPosition;
        private Vector3 _target;
        public void Init(ShapeDescription description, Color color)
        {
            _pathToFollow = description.pathToFollow;
            _spriteRenderer.color = color;
            TimeToPress = description.timeToPress;
            _speed = description.speed;

            _startPosition = _pathToFollow[0];
            _target = _pathToFollow[1];
            _t = 0;
            _i = 2;
            // Debug.Log( "Init --> _startPosition : "+_startPosition+", _target :"+_target);
        }
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Update() 
        {
            transform.position = Vector3.MoveTowards(_startPosition, _target, _t);
            // Debug.Log("_i" + _i + "_pathToFollow.Length" + _pathToFollow.Length+ "transform.position "+ transform.position +"_target"+_target);
            if (_i < _pathToFollow.Length && transform.position == _target)
            {
                SetDestination(_pathToFollow[_i]);
                _i++;
            }
            _t += Time.deltaTime * _speed;
        }
    
        private void SetDestination(Vector3 destination)
        {
            _t = 0;
            _startPosition = _target;
            _target = destination;
            // Debug.Log( "SetDestination --> _startPosition : "+_startPosition+", _target :"+_target);
        }
        
    }
}