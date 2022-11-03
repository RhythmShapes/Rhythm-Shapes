﻿using System;
using models;
using shape;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<Target> onInputPerformed;
    [SerializeField] private UnityEvent onGamePaused;
    [SerializeField] private UnityEvent onGameUnpaused;
    
    private InputSystem _inputSystem;

    private void Awake()
    {
        onInputPerformed ??= new UnityEvent<Target>();
        onGamePaused ??= new UnityEvent();
        onGameUnpaused ??= new UnityEvent();
    }

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
        _inputSystem.Player.Top.performed += _ => InputPerformed(Target.Top);
        _inputSystem.Player.Left.performed += _ => InputPerformed(Target.Left);
        _inputSystem.Player.Right.performed += _ => InputPerformed(Target.Right);
        _inputSystem.Player.Bottom.performed += _ => InputPerformed(Target.Bottom);
        _inputSystem.Player.Pause.performed += _ => PausePerformed();
        _inputSystem.UI.UnPause.performed += _ => UnPausePerformed();
    }

    private void InputPerformed(Target target)
    {
        if (GameModel.Instance.HasNextAttendedInput())
        {
            GameModel.Instance.GetNextAttendedInput().SetPressed(target);
            onInputPerformed.Invoke(target);
        }
    }
    
    private void PausePerformed()
    {
        
        if (SceneManager.GetActiveScene().buildIndex == 1) //SceneManager.GetActiveScene().buildIndex == 1
        {
            Debug.Log("PausePerformed");
            onGamePaused.Invoke();
        }
    }

    private void UnPausePerformed()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("UnPausePerformed");
            onGameUnpaused.Invoke();
        }
    }

    public void EnableInputSystemUI()
    {
        _inputSystem.Player.Disable();
        _inputSystem.UI.Enable();
    }
    
    public void DisableInputSystemUI()
    {
        _inputSystem.UI.Disable();
        _inputSystem.Player.Enable();
    }
}