using System;
using UnityEngine;
using utils.XML;

namespace shape
{
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class Shape : MonoBehaviour
    {
        [SerializeField] private ShapeType type;
        public ShapeType Type { get; private set; }
        public float TimeToPress { get; private set; }
        public float TimeToSpawn { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private Vector2[] _pathToFollow;
        private float _speed;
    

        float t;
        private int i = 2;
        Vector3 startPosition;
        Vector3 target;
        float timeToReachTarget;
        private float _totalPathDistance;
        public void Init(ShapeDescription description, Color color)
        {
            _pathToFollow = description.pathToFollow;
            _spriteRenderer.color = color;
            TimeToPress = description.timeToPress;
            _speed = description.speed;
            for (int i = 0; i < _pathToFollow.Length-1; i++)
            {
                _totalPathDistance += Vector3.Distance(_pathToFollow[i], _pathToFollow[i + 1]);
            }
            Debug.Log("TotalPathDistance : " + _totalPathDistance);
            TimeToSpawn = TimeToPress - _totalPathDistance / _speed;
            startPosition = _pathToFollow[0];
            target = _pathToFollow[1];
            timeToReachTarget = TimeToPress / _pathToFollow.Length;
        }
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    
        void Update() 
        {
            t += Time.deltaTime/timeToReachTarget;
            transform.position = Vector3.Lerp(startPosition, target, t);
            if (i < _pathToFollow.Length && transform.position == target)
            {
                SetDestination(_pathToFollow[i]);
                i++;
            }
        }
    
        private void SetDestination(Vector3 destination)
        {
            t = 0;
            startPosition = transform.position;
            target = destination; 
        }
    
        // private float _totalPathDistance;
        // private Vector3 _startPosition;
        // private Vector3 _target;
        // private float t;
        // private int i = 2;
        // private float distanceFraction;
        // public void Init(ShapeDescription description)
        // {
        //     _pathToFollow = description.pathToFollow;
        //     _spriteRenderer.color = description.color;
        //     TimeToPress = description.timeToPress;
        //     _speed = description.speed;
        //
        //     for (int i = 0; i < _pathToFollow.Length-1; i++)
        //     {
        //         _totalPathDistance += Vector3.Distance(_pathToFollow[i], _pathToFollow[i + 1]);
        //     }
        //
        //     TimeToSpawn = TimeToPress - _totalPathDistance / _speed;
        //     _startPosition = _pathToFollow[0];
        //     _target = _pathToFollow[1];
        //     distanceFraction = ((TimeToPress - TimeToSpawn) * _speed)/_pathToFollow.Length;
        // }
        //
        // private void Awake()
        // {
        //     _spriteRenderer = GetComponent<SpriteRenderer>();
        // }
        //
        // void Update() 
        // {
        //     
        // }
        //
        // private void SetDestination(Vector3 destination)
        // {
        //     
        // }
    }
}