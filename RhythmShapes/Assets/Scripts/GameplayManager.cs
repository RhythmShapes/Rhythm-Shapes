using System;
using System.Collections.Generic;
using shape;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using utils.XML;
using Cache = UnityEngine.Cache;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    private PlayerInputAction _playerInputAction;
    public Queue<Shape> _topTargetQueue;
    public Queue<Shape> _leftTargetQueue;
    public Queue<Shape> _rightTargetQueue;
    public Queue<Shape> _bottomTargetQueue;
    

    private AudioSource _audioSource;
    // private AudioClip _audioClip;
    public float globalTime = -5;
    public float goodWindow = 0.1f;
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
    
    private void Update()
    {
        globalTime += Time.deltaTime;
        // Debug.Log("globalTime : " + globalTime);
        
        if (/*Input.GetKey(KeyCode.P) && */!_audioSource.isPlaying && globalTime >= 0)
        {
            StartMusic();
        }

        if (_audioSource.isPlaying)
        {
            Debug.Log(_audioSource.time);
        }
        
        ShapesSpawner.Instance.SpawnShapes(globalTime,Time.deltaTime);
        ShapesReleaser.Instance.ReleaseIfOutOfTime();

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
    
    private void TopPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("TopPerformed : " + context);
        if (_topTargetQueue.Count != 0)
        {
            if (Validation.Instance.TestInput(_topTargetQueue))
            {
                UIManager.Instance.SetBlueTextGood();
            }
        }
 
    }
    
    private void LeftPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("LeftPerformed : " + context);
        if (_leftTargetQueue.Count != 0)
        {
            if (Validation.Instance.TestInput(_leftTargetQueue))
            {
                UIManager.Instance.SetGreenTextGood();
            }
        }
        
    }
    
    private void RightPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("RightPerformed : " + context);
        if (_rightTargetQueue.Count != 0)
        {
            if (Validation.Instance.TestInput(_rightTargetQueue))
            {
                UIManager.Instance.SetRedTextGood();
            }
        }
    }
    
    private void BottomPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("BottomPerformed : " + context);
        if (_bottomTargetQueue.Count !=0)
        {
            if (Validation.Instance.TestInput(_bottomTargetQueue))
            {
                UIManager.Instance.SetYellowTextGood();
            }
        }    
        
    }
    
    public void TestingPlayerInputTriggered(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered" + context);
    }
    
}
