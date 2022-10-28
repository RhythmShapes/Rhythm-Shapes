using System;
using System.Collections.Generic;
using shape;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using utils.XML;
using Cache = UnityEngine.Cache;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    private PlayerInputAction _playerInputAction;
    private Queue<Shape> _topTargetQueue;
    private Queue<Shape> _leftTargetQueue;
    private Queue<Shape> _rightTargetQueue;
    private Queue<Shape> _bottomTargetQueue;

    private Queue<ShapeDescription> _shapeDescriptionsQueue;
    private Queue<float> _spawnTimesQueue;
    private Queue<Color> _shapeColorsQueue;
    private Queue<Queue<float>> _timeToTargetQueues;

    private AudioSource _audioSource;
    // private AudioClip _audioClip;
    private float _globaleTime = -5;
    private float _goodWindow = 0.2f;
    private float _tapTime;
    
    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
        
        _playerInputAction = new PlayerInputAction();
        _playerInputAction.Player.Enable();
        _playerInputAction.Player.Top.performed += TopPerformed;
        _playerInputAction.Player.Left.performed += LeftPerformed;
        _playerInputAction.Player.Right.performed += RightPerformed;
        _playerInputAction.Player.Bottom.performed += BottomPerformed;
        _audioSource = GetComponent<AudioSource>();
        
        _topTargetQueue = new Queue<Shape>();
        _leftTargetQueue = new Queue<Shape>();
        _rightTargetQueue = new Queue<Shape>();
        _bottomTargetQueue = new Queue<Shape>();

        _timeToTargetQueues = new Queue<Queue<float>>();

        _shapeDescriptionsQueue = new Queue<ShapeDescription>();
        _spawnTimesQueue = new Queue<float>();
        _shapeColorsQueue = new Queue<Color>();
    }
    
    
    public void Init(LevelDescription level)
    {
        Debug.Log("levelTitle : " + level.title);
        for (int i = 0; i < level.shapes.Length; i++)
        {
            ShapeDescription shapeDescription = level.shapes[i];
            float speedAdjustment;
            // Debug.Log("iteration :" + i);
            // _spawnTimes.Enqueue(level.shapes[i].timeToPress);
            _shapeDescriptionsQueue.Enqueue(shapeDescription);
            _shapeColorsQueue.Enqueue(level.GetTargetColor(shapeDescription.target));
            
            float totalPathDistance = ComputeShapeElement(shapeDescription); 
            Debug.Log("shapeDescription.pathToFollow.Length : " + shapeDescription.pathToFollow.Length);
            switch (shapeDescription.type) // speedAdjustment according to shape
            {
                case ShapeType.Square :
                    speedAdjustment = 14.48528f/8.485281f; 
                    shapeDescription.speed *= speedAdjustment;
                    break;
                case ShapeType.Circle :
                    speedAdjustment = 10.7225f/8.485281f;
                    shapeDescription.speed *= speedAdjustment;
                    break;
                case ShapeType.Diamond :
                    speedAdjustment = 1;
                    shapeDescription.speed *= speedAdjustment;
                    break;
                default:
                    Debug.LogError("Unknown Type, using Square as default");
                    speedAdjustment = 14.48528f/8.485281f;
                    shapeDescription.speed *= speedAdjustment;
                    break;
            }
            // Debug.Log("TotalPathDistance : " + totalPathDistance);
            // Debug.Log("SpawnTimes : " + (shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed));
            _spawnTimesQueue.Enqueue(shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed);
        }
    }

    private float ComputeShapeElement(ShapeDescription shapeDescription)
    {
        float totalPathDistance = 0;
        Vector2 lastElement = Vector2.zero;
        Queue<float> timeTotargetQueue = new Queue<float>();
        
        foreach (var element in shapeDescription.pathToFollow)
        {
            float distanceI = Vector3.Distance(
                lastElement, 
                element);
            totalPathDistance +=
                distanceI;
            float timeI = distanceI / shapeDescription.speed;
            timeTotargetQueue.Enqueue(timeI);
            // Debug.Log("CalculatePathLength -> lastElement : "+lastElement+", element : " + element);
            lastElement = element;
        }

        _timeToTargetQueues.Enqueue(timeTotargetQueue);
        return totalPathDistance;
    }
    
    

    private void SpawnShapes(float globalTime, float deltaTime)
    {
        float precision = deltaTime/2;
        Shape shape;
        ShapeDescription nextSpawnableShape = _shapeDescriptionsQueue.Peek();
        float nextSpawnableShapeSpawnTime = _spawnTimesQueue.Peek();
        Color nextSpawnableShapeColor = _shapeColorsQueue.Peek();
        // Debug.Log(globalTime);

        if (nextSpawnableShapeSpawnTime > globalTime-precision && nextSpawnableShapeSpawnTime < globalTime+precision)
        {
            // Debug.Log("SpawnShapes -> globalTime : " + globalTime);
            shape = ShapeFactory.Instance.GetShape(nextSpawnableShape.type);
            shape.transform.position = Vector3.zero;
            shape.Init(nextSpawnableShape,nextSpawnableShapeColor,_timeToTargetQueues.Dequeue());

            switch (nextSpawnableShape.target)
            {
                case Target.Top:
                    _topTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Bottom:
                    _bottomTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Right:
                    _rightTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Left:
                    _leftTargetQueue.Enqueue(shape);
                    break;
            
                default:
                    Debug.LogError("Unknown Target, using Top as default");
                    _topTargetQueue.Enqueue(shape);
                    break;
            }

            _shapeDescriptionsQueue.Dequeue();
            _spawnTimesQueue.Dequeue();
            _shapeColorsQueue.Dequeue();
        }
    }
    private void Update()
    {
        _globaleTime += Time.deltaTime;
        // Debug.Log("_globalTime : " + _globaleTime);
        
        if (/*Input.GetKey(KeyCode.P) && */!_audioSource.isPlaying && _globaleTime >= 0)
        {
            StartMusic();
        }

        if (_audioSource.isPlaying)
        {
            Debug.Log(_audioSource.time);
        }
        
        SpawnShapes(_globaleTime,Time.deltaTime);
        ReleaseIfOutOfTime();

    }

    private void StartMusic()
    {
        if (_audioSource.clip == null)
        {
            Debug.LogError("NoMusic");
        }
        else
        {
            // Debug.Log("StartMusic" + _audioSource.clip.name);
            _audioSource.Play();
        }
        
    }

    private void ReleaseIfOutOfTime()
    {
        if (_topTargetQueue.Count == 0 && 
            _leftTargetQueue.Count == 0 && 
            _rightTargetQueue.Count == 0 &&
            _bottomTargetQueue.Count == 0)
        {
            return;
        }
        else
        {
            float topTime = 10000000;
            float leftTime= 10000000;
            float rightTime = 10000000;
            float bottomTime = 10000000;

            if (_topTargetQueue.Count != 0)
            {
                topTime = _topTargetQueue.Peek().TimeToPress;
            }
        
            if (_leftTargetQueue.Count != 0)
            {
                leftTime = _leftTargetQueue.Peek().TimeToPress;
            }
            if (_rightTargetQueue.Count != 0)
            {
                rightTime = _rightTargetQueue.Peek().TimeToPress;
            }
            if (_bottomTargetQueue.Count != 0)
            {
                bottomTime = _bottomTargetQueue.Peek().TimeToPress;
            }

            float min = Mathf.Min(topTime, leftTime, rightTime, bottomTime);
            // Debug.Log("ReleaseIfOutOfTime -> Min : " + min);
            if (min == topTime && _globaleTime >  topTime + _goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> topTime : " + min);
                ShapeFactory.Instance.Release(_topTargetQueue.Dequeue());
            }
            else if (min == leftTime && _globaleTime >  leftTime + _goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> leftTime : " + min);
                ShapeFactory.Instance.Release(_leftTargetQueue.Dequeue());
            }
            else if (min == rightTime && _globaleTime >  rightTime + _goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> rightTime : " + min);
                ShapeFactory.Instance.Release(_rightTargetQueue.Dequeue());
            }
            else if (min == bottomTime && _globaleTime >  bottomTime + _goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> bottomTime : " + min);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
            // else
            // {
            //     Debug.Log("Not supposed to happened");
            // }
        }
    }
    private void TopPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("TopPerformed : " + context);

        
        if (_topTargetQueue.Count != 0)
        {
            Shape currentShape = _topTargetQueue.Peek();
            // _tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Top : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_topTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Top : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_topTargetQueue.Dequeue());
            }
        }
 
    }
    
    private void LeftPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("LeftPerformed : " + context);
        if (_leftTargetQueue.Count != 0)
        {
            Shape currentShape = _leftTargetQueue.Peek();
            // _tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Left : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_leftTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Left : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_leftTargetQueue.Dequeue());
            }
        }
        
    }
    
    private void RightPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("RightPerformed : " + context);

        if (_rightTargetQueue.Count != 0)
        {
            Shape currentShape = _rightTargetQueue.Peek();
            // _tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Right : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_rightTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Right : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_rightTargetQueue.Dequeue());
            }
        }
    }
    
    private void BottomPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("BottomPerformed : " + context);

        if (_bottomTargetQueue.Count !=0)
        {
            Shape currentShape = _bottomTargetQueue.Peek();
            // _tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Bottom : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Bottom : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
        }    
        
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }
    
}
