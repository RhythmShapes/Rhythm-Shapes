using System;
using models;
using shape;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _inputSystem.UI.Enable();
        }
        else
        {
            _inputSystem.Player.Enable();
            _inputSystem.Player.Top.performed += PerformTop;
            _inputSystem.Player.Left.performed += PerformLeft;
            _inputSystem.Player.Right.performed += PerformRight;
            _inputSystem.Player.Bottom.performed += PerformBottom;
            _inputSystem.Player.Top.canceled += CancelTop;
            _inputSystem.Player.Left.canceled += CancelLeft;
            _inputSystem.Player.Right.canceled += CancelRight;
            _inputSystem.Player.Bottom.canceled += CancelBottom;
            _inputSystem.Player.Pause.performed += PausePerformed;
            _inputSystem.UI.UnPause.performed += UnPausePerformed;
        }
        
    }

    private void OnDisable()
    {
        _inputSystem.Player.Top.performed -= PerformTop;
        _inputSystem.Player.Left.performed -= PerformLeft;
        _inputSystem.Player.Right.performed -= PerformRight;
        _inputSystem.Player.Bottom.performed -= PerformBottom;
        _inputSystem.Player.Top.canceled -= CancelTop;
        _inputSystem.Player.Left.canceled -= CancelLeft;
        _inputSystem.Player.Right.canceled -= CancelRight;
        _inputSystem.Player.Bottom.canceled -= CancelBottom;
        _inputSystem.Player.Pause.performed -= PausePerformed;
        _inputSystem.UI.UnPause.performed -= UnPausePerformed;
    }

    private void PerformTop(InputAction.CallbackContext callbackContext)
    {
        InputPerformed(Target.Top);
    }

    private void PerformLeft(InputAction.CallbackContext callbackContext)
    {
        InputPerformed(Target.Left);
    }

    private void PerformRight(InputAction.CallbackContext callbackContext)
    {
        InputPerformed(Target.Right);
    }

    private void PerformBottom(InputAction.CallbackContext callbackContext)
    {
        InputPerformed(Target.Bottom);
    }

    private void CancelTop(InputAction.CallbackContext callbackContext)
    {
        InputCanceled(Target.Top);
    }

    private void CancelLeft(InputAction.CallbackContext callbackContext)
    {
        InputCanceled(Target.Left);
    }

    private void CancelRight(InputAction.CallbackContext callbackContext)
    {
        InputCanceled(Target.Right);
    }

    private void CancelBottom(InputAction.CallbackContext callbackContext)
    {
        InputCanceled(Target.Bottom);
    }

    private void InputPerformed(Target target)
    {
        GetComponent<TargetLightOnKeyPress>().On(target);
        if (GameModel.Instance.HasNextAttendedInput())
        {
            GameModel.Instance.GetNextAttendedInput().SetPressed(target);
            onInputPerformed.Invoke(target);
            // Debug.Log("InputManager -> InputPerformed : " + gameObject.GetComponent<AudioSource>().time);
            if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
            {
                var audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    TestingCalibration.Instance.EnqueueInputReceivedTimeQueue(audioSource.time);
                }
                
                
            }
        }
    }
    
    private void InputCanceled(Target target)
    {
        GetComponent<TargetLightOnKeyPress>().Off(target);
        if (GameModel.Instance.HasNextAttendedInput())
        {
            GameModel.Instance.GetNextAttendedInput().SetPressed(target,false);
            // onInputPerformed.Invoke(target);
        }
    }
    
    private void PausePerformed(InputAction.CallbackContext callbackContext)
    {
        
        if (!GameModel.Instance.isGamePaused)
        {
            Debug.Log("PausePerformed");
            onGamePaused.Invoke();
        }
    }

    private void UnPausePerformed(InputAction.CallbackContext callbackContext)
    {
        if (GameModel.Instance.isGamePaused)
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