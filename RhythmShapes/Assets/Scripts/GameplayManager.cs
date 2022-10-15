using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using XML;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    private PlayerInputAction _playerInputAction;
    private Queue<Shape> _blueShapeQueue;
    private Queue<Shape> _greenShapeQueue;
    private Queue<Shape> _redShapeQueue;
    private Queue<Shape> _yellowShapeQueue;
    
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
        _playerInputAction.Player.Blue.performed += BluePerformed;
        _playerInputAction.Player.Green.performed += GreenPerformed;
        _playerInputAction.Player.Red.performed += RedPerformed;
        _playerInputAction.Player.Yellow.performed += YellowPerformed;
        _audioSource = GetComponent<AudioSource>();

        _blueShapeQueue = new Queue<Shape>();
        _greenShapeQueue = new Queue<Shape>();
        _redShapeQueue = new Queue<Shape>();
        _yellowShapeQueue = new Queue<Shape>();
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }

    public void Init(LevelDescription level)
    {
        // level.title
        Debug.Log("levelTitle : " + level.title);
        // level.shapes
        Shape shape;

        for (int i = 0; i < level.shapes.Length; i++)
        {
            Debug.Log("iteration :" + i);
            //_spawnTimes.Enqueue(level.shapes[i].timeToPress);
            shape = ShapeFactory.Instance.GetShape(level.shapes[i].type);
            shape.Init(level.shapes[i]);
            Debug.Log(shape.TimeToSpawn);
            
            //Debug.Log(level.shapes[i].pathToFollow + ", length : " + level.shapes[i].pathToFollow.Length );

            for (int j = 0; j < level.shapes[i].pathToFollow.Length ; j++)
            {
                //Debug.Log(level.shapes[i].pathToFollow[j]);
            }
            if (CompareColor(level.shapes[i].color, Color.blue))
                _blueShapeQueue.Enqueue(shape);
            else if (CompareColor(level.shapes[i].color, Color.red))
                _redShapeQueue.Enqueue(shape);
            else if (CompareColor(level.shapes[i].color, Color.green))
                _greenShapeQueue.Enqueue(shape);
            else if (CompareColor(level.shapes[i].color, Color.yellow))
                _yellowShapeQueue.Enqueue(shape);
            else
            {
                Debug.LogError("Unknown color, using blue as default");
                _blueShapeQueue.Enqueue(shape);
            }
        }
        // shape.pathToFollow
        // ...
        Debug.Log(_blueShapeQueue + ", " + _redShapeQueue + ", " +_greenShapeQueue +", " +_yellowShapeQueue);
        
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

    private void BluePerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("BluePerformed : " + context);

        
        if (_blueShapeQueue.Count != 0)
        {
            Shape currentShape = _blueShapeQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Blue : GOOOOOOOD");
                ShapeFactory.Instance.Release(_blueShapeQueue.Dequeue());
            }
            else
            {
                Debug.Log("Blue : FAILED");
            }
        }
 
    }
    
    private void GreenPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("GreenPerformed : " + context);
        if (_greenShapeQueue.Count != 0)
        {
            Shape currentShape = _greenShapeQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Green : GOOOOOOOD");
                ShapeFactory.Instance.Release(_greenShapeQueue.Dequeue());
            }
            else
            {
                Debug.Log("Green : FAILED");
            }
        }
        
    }
    
    private void RedPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("RedPerformed : " + context);

        if (_redShapeQueue.Count != 0)
        {
            Shape currentShape = _redShapeQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Red : GOOOOOOOD");
                ShapeFactory.Instance.Release(_redShapeQueue.Dequeue());
            }
            else
            {
                Debug.Log("Red : FAILED");
            }
        }
    }
    
    private void YellowPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("YellowPerformed : " + context);

        if (_yellowShapeQueue.Count !=0)
        {
            Shape currentShape = _yellowShapeQueue.Peek();
            _tapTime = _audioSource.time;
            if (_tapTime > currentShape.TimeToPress - _goodWindow || _tapTime < currentShape.TimeToPress + _goodWindow)
            {
                Debug.Log("Yellow : GOOOOOOOD");
                ShapeFactory.Instance.Release(_yellowShapeQueue.Dequeue());
            }
            else
            {
                Debug.Log("Yellow : FAILED");
            }
        }    
        
    }
    
    private bool CompareColor(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
    }
    
}
