using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    private PlayerInputAction _playerInputAction;
    private Queue<Shape> _blueShapeQueue;
    private Queue<Shape> _greenShapeQueue;
    private Queue<Shape> _redShapeQueue;
    private Queue<Shape> _yellowShapeQueue;
    
    private AudioSource _audioSource;
    //private AudioClip _audioClip;
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
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("BluePerformed : " + context);

        Shape currentShape = _blueShapeQueue.Peek();
        if (currentShape.TimeToPress == _audioSource.time)
        {
            ShapeFactory.Instance.Release(_blueShapeQueue.Dequeue());
        }
        
    }
    
    private void GreenPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("GreenPerformed : " + context);
        
        Shape currentShape = _blueShapeQueue.Peek();
        if (currentShape.TimeToPress == _audioSource.time)
        {
            ShapeFactory.Instance.Release(_blueShapeQueue.Dequeue());
        }
    }
    
    private void RedPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("RedPerformed : " + context);
    }
    
    private void YellowPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("YellowPerformed : " + context);
    }
    
}
