using System.Collections.Generic;
using shape;
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
    
    //private Queue<float> _spawnTimes;
    //private Queue<Shape> _colors;
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
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }

    public void Init(LevelDescription level)
    {
        Debug.Log("levelTitle : " + level.title);
        Shape shape;

        for (int i = 0; i < level.shapes.Length; i++)
        {
            Debug.Log("iteration :" + i);
            //_spawnTimes.Enqueue(level.shapes[i].timeToPress);
            shape = ShapeFactory.Instance.GetShape(level.shapes[i].type);
            shape.Init(level.shapes[i], level.GetTargetColor(level.shapes[i].target));
            Debug.Log(shape.TimeToSpawn);
            
            //Debug.Log(level.shapes[i].pathToFollow + ", length : " + level.shapes[i].pathToFollow.Length );

            for (int j = 0; j < level.shapes[i].pathToFollow.Length ; j++)
            {
                //Debug.Log(level.shapes[i].pathToFollow[j]);
            }
            
            switch (level.shapes[i].target)
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
        }
        Debug.Log(_topTargetQueue + ", " + _rightTargetQueue + ", " +_leftTargetQueue +", " +_bottomTargetQueue);
        
    }

    private void Update()
    {
        _globaleTime += Time.deltaTime;
        Debug.Log("_globalTime : " + _globaleTime);
        if (Input.GetKey(KeyCode.P) && !_audioSource.isPlaying)
        {
            StartMusic();
        }

        if (_audioSource.isPlaying)
        {
            Debug.Log(_audioSource.time);
        }
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

    private void TopPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("BluePerformed : " + context);

        
        if (_topTargetQueue.Count != 0)
        {
            Shape currentShape = _topTargetQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Blue : GOOOOOOOD");
                ShapeFactory.Instance.Release(_topTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Blue : FAILED");
            }
        }
 
    }
    
    private void LeftPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("GreenPerformed : " + context);
        if (_leftTargetQueue.Count != 0)
        {
            Shape currentShape = _leftTargetQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Green : GOOOOOOOD");
                ShapeFactory.Instance.Release(_leftTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Green : FAILED");
            }
        }
        
    }
    
    private void RightPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("RedPerformed : " + context);

        if (_rightTargetQueue.Count != 0)
        {
            Shape currentShape = _rightTargetQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Red : GOOOOOOOD");
                ShapeFactory.Instance.Release(_rightTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Red : FAILED");
            }
        }
    }
    
    private void BottomPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("YellowPerformed : " + context);

        if (_bottomTargetQueue.Count !=0)
        {
            Shape currentShape = _bottomTargetQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Yellow : GOOOOOOOD");
                ShapeFactory.Instance.Release(_bottomTargetQueue.Dequeue());
            }
            else
            {
                Debug.Log("Yellow : FAILED");
            }
        }    
        
    }
    
}
