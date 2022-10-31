using shape;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onInputPerformed;
    
    private InputSystem _inputSystem;
    
    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
        _inputSystem.Player.Top.performed += _ => InputPerformed(Target.Top);
        _inputSystem.Player.Left.performed += _ => InputPerformed(Target.Left);
        _inputSystem.Player.Right.performed += _ => InputPerformed(Target.Right);
        _inputSystem.Player.Bottom.performed += _ => InputPerformed(Target.Bottom);
    }

    private void InputPerformed(Target target)
    {
        if (GameModel.Instance.HasNextAttendedInput())
        {
            GameModel.Instance.GetNextAttendedInput().SetPressed(target);
            onInputPerformed.Invoke();
        }
    }
}