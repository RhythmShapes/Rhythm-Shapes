using System.Collections.Generic;
using shape;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    [SerializeField] private float goodPressedWindow;
    
    public static GameModel Instance { get; private set; }
    public float GoodPressedWindow => goodPressedWindow;

    private readonly Queue<ShapeModel> _shapeModels = new();
    private readonly Queue<AttendedInput> _attendedInputs = new();

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    public bool HasNextShapeModel()
    {
        return _shapeModels.Count > 0;
    }

    public void AddShapeModel(ShapeModel shape)
    {
        _shapeModels.Enqueue(shape);
    }

    public ShapeModel GetNextShapeModel()
    {
        if (!_shapeModels.TryPeek(out var shape))
            return null;
        
        return shape;
    }

    public void PopShapeDescription()
    {
        if(_shapeModels.Count > 0)
            _shapeModels.Dequeue();
    }

    public bool HasNextAttendedInput()
    {
        return _attendedInputs.Count > 0;
    }

    public void AddAttendedInput(AttendedInput input)
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