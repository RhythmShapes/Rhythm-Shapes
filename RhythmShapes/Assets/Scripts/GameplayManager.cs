using System.Collections.Generic;
using shape;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using utils.XML;

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

    private AudioSource _audioSource;
    //private AudioClip _audioClip;
    private float _globaleTime;
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

        _shapeDescriptionsQueue = new Queue<ShapeDescription>();
        _spawnTimesQueue = new Queue<float>();
        _shapeColorsQueue = new Queue<Color>();
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }

    public void Init(LevelDescription level)
    {
        Debug.Log("levelTitle : " + level.title);
        for (int i = 0; i < level.shapes.Length; i++)
        {
            ShapeDescription shapeDescription;
            Debug.Log("iteration :" + i);
            //_spawnTimes.Enqueue(level.shapes[i].timeToPress);
            shapeDescription = level.shapes[i];
            _shapeDescriptionsQueue.Enqueue(shapeDescription);
            _shapeColorsQueue.Enqueue(level.GetTargetColor(shapeDescription.target));
            
            float totalPathDistance = 0; 
            for (int j = 0; j < shapeDescription.pathToFollow.Length-1; j++)
            {
                totalPathDistance += 
                    Vector3.Distance(
                        shapeDescription.pathToFollow[j], 
                        shapeDescription.pathToFollow[j + 1]);
            }
            Debug.Log("TotalPathDistance : " + totalPathDistance);
            Debug.Log("SpawnTimes : " + (shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed));
            _spawnTimesQueue.Enqueue(shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed);
        }
    }

    private void SpawnShapes(float globalTime, float deltaTime)
    {
        float precision = deltaTime/2;
        Shape shape;
        ShapeDescription nextSpawnableShape = _shapeDescriptionsQueue.Peek();
        float nextSpawnableShapeSpawnTime = _spawnTimesQueue.Peek();
        Color nextSpawnableShapeColor = _shapeColorsQueue.Peek();
        //Debug.Log(globalTime);

        if (nextSpawnableShapeSpawnTime > globalTime-precision && nextSpawnableShapeSpawnTime < globalTime+precision)
        {
            Debug.Log("SpawnShapes -> globalTime : " + globalTime);
            shape = ShapeFactory.Instance.GetShape(nextSpawnableShape.type);
            shape.Init(nextSpawnableShape,nextSpawnableShapeColor);
            Debug.Log("SpawnShapes -> TimeToSpawn : " + shape.TimeToSpawn);

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
        //Debug.Log("_globalTime : " + _globaleTime);
        
        if (Input.GetKey(KeyCode.P) && !_audioSource.isPlaying)
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
            Debug.Log("NoMusic");
        }
        else
        {
            //Debug.Log("StartMusic" + _audioSource.clip.name);
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
                Debug.Log("ReleaseIfOutOfTime -> topTime : " + min);
                ShapeFactory.Instance.Release(_topTargetQueue.Peek());
            }
            else if (min == leftTime && _globaleTime >  leftTime + _goodWindow)
            {
                ShapeFactory.Instance.Release(_leftTargetQueue.Peek());
            }
            else if (min == rightTime && _globaleTime >  rightTime + _goodWindow)
            {
                ShapeFactory.Instance.Release(_rightTargetQueue.Peek());
            }
            else if (min == bottomTime && _globaleTime >  bottomTime + _goodWindow)
            {
                ShapeFactory.Instance.Release(_bottomTargetQueue.Peek());
            }
            else
            {
                Debug.Log("Not supposed to happened");
            }
        }
    }
    private void TopPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("BluePerformed : " + context);

        
        if (_topTargetQueue.Count != 0)
        {
            Shape currentShape = _topTargetQueue.Peek();
            //_tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Top : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_topTargetQueue.Dequeue());
            }
            else if (_globaleTime >  currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Top : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
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
        //Debug.Log("GreenPerformed : " + context);
        if (_leftTargetQueue.Count != 0)
        {
            Shape currentShape = _leftTargetQueue.Peek();
            //_tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Left : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_leftTargetQueue.Dequeue());
            }
            else if (_globaleTime >  currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Left : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
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
        //Debug.Log("RedPerformed : " + context);

        if (_rightTargetQueue.Count != 0)
        {
            Shape currentShape = _rightTargetQueue.Peek();
            //_tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Right : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_rightTargetQueue.Dequeue());
            }
            else if (_globaleTime >  currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Right : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
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
        //Debug.Log("YellowPerformed : " + context);

        if (_bottomTargetQueue.Count !=0)
        {
            Shape currentShape = _bottomTargetQueue.Peek();
            //_tapTime = _audioSource.time;
            _tapTime = _globaleTime;
            if (_tapTime > currentShape.TimeToPress - _goodWindow && _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Bottom : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
            else if (_globaleTime >  currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Bottom : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Bottom : FAILED, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
        }    
        
    }
    
}
