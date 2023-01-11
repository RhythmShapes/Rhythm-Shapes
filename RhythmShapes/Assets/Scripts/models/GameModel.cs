using System.Collections.Generic;
using shape;
using UnityEngine;
using UnityEngine.Events;

namespace models
{
    public class GameModel : MonoBehaviour
    {
        [SerializeField] private float perfectPressedWindow;
        [SerializeField] private float goodPressedWindow;
        [SerializeField] private float okPressedWindow;
        [SerializeField] private float badPressedWindow;
        
        [SerializeField] private UnityEvent onGamePaused;
        [SerializeField] private UnityEvent onGameUnPaused;
        [SerializeField] private UnityEvent onGameRestarted;
        public float PerfectPressedWindow => perfectPressedWindow;
        public float GoodPressedWindow => goodPressedWindow;
        public float OkPressedWindow => okPressedWindow;
        public float BadPressedWindow => badPressedWindow;

        public bool isGamePaused = true;
        public static GameModel Instance { get; private set; }

        private Queue<ShapeModel> _shapeModels = new();
        private Queue<AttendedInput> _attendedInputs = new();

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
            
            onGamePaused ??= new UnityEvent();
            onGameUnPaused ??= new UnityEvent();
            onGameRestarted ??= new UnityEvent();
        }

        public void PauseGame()
        {
            isGamePaused = true;
            onGamePaused.Invoke();
        }

        public void UnPauseGame()
        {
            onGameUnPaused.Invoke();
        }
        
        public void RestartGame()
        {
            isGamePaused = true;
            onGameRestarted.Invoke();
        }

        public void SetGamePauseStateTrue()
        {
            isGamePaused = true;
        }
        
        public void SetGamePauseStateFalse()
        {
            isGamePaused = false;
        }
        public void RefreshQueue()
        {
            _shapeModels.Clear();
            _attendedInputs.Clear();
            _shapeModels = new();
            _attendedInputs = new();

        }
        public bool HasNextShapeModel()
        {
            return _shapeModels.Count > 0;
        }

        public void PushShapeModel(ShapeModel shape)
        {
            _shapeModels.Enqueue(shape);
        }

        public ShapeModel GetNextShapeModel()
        {
            if (!_shapeModels.TryPeek(out var shape))
                return null;
        
            return shape;
        }

        public void PopShapeModel()
        {
            if(_shapeModels.Count > 0)
                _shapeModels.Dequeue();
        }

        public bool HasNextAttendedInput()
        {
            return _attendedInputs.Count > 0;
        }

        public void PushAttendedInput(AttendedInput input)
        {
            _attendedInputs.Enqueue(input);
        }

        public AttendedInput GetNextAttendedInput()
        {
            if (!_attendedInputs.TryPeek(out var input))
                return null;
        
            return input;
        }

        public void PopAttendedInput()
        {
            if(_attendedInputs.Count > 0)
                _attendedInputs.Dequeue();
        }
    }
}