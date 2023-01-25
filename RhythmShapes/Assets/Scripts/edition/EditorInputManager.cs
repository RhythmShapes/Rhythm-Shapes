using UnityEngine;
using UnityEngine.Events;

namespace edition
{
    public class EditorInputManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSave;
        [SerializeField] private UnityEvent onQuit;
        [SerializeField] private UnityEvent onDelete;
        
        private const int KeyQ = 113;
        private const int KeyS = 115;
    
        private InputSystem _inputSystem;
        private bool _controlPressed;

        private void Awake()
        {
            onSave ??= new UnityEvent();
            onQuit ??= new UnityEvent();
            onDelete ??= new UnityEvent();
        }

        private void OnEnable()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Editor.Enable();
            _inputSystem.Editor.Control.performed += _ => OnControlPressed(true);
            _inputSystem.Editor.Delete.performed += _ => OnDelete();
            _inputSystem.Editor.KeyS.performed += _ => OnKeyPressed(KeyS);
            _inputSystem.Editor.KeyQ.performed += _ => OnKeyPressed(KeyQ);
            _inputSystem.Editor.Control.canceled += _ => OnControlPressed(false);
            _inputSystem.Editor.Delete.canceled += _ => OnDelete();
        }

        private void OnDisable()
        {
            _inputSystem.Editor.Control.performed -= _ => OnControlPressed(true);
            _inputSystem.Editor.Delete.performed -= _ => OnDelete();
            _inputSystem.Editor.KeyS.performed -= _ => OnKeyPressed(KeyS);
            _inputSystem.Editor.KeyQ.performed -= _ => OnKeyPressed(KeyQ);
            _inputSystem.Editor.Control.canceled -= _ => OnControlPressed(false);
            _inputSystem.Editor.Delete.canceled -= _ => OnDelete();
        }

        private void OnControlPressed(bool pressed)
        {
            _controlPressed = pressed;
        }

        private void OnKeyPressed(int key)
        {
            if(!_controlPressed) return;

            switch (key)
            {
                case KeyS:
                    onSave.Invoke();
                    break;
                case KeyQ:
                    onQuit.Invoke();
                    break;
            }
        }

        private void OnDelete()
        {
            if(EditorModel.IsInspectingShape())
                onDelete.Invoke();
        }
    }
}