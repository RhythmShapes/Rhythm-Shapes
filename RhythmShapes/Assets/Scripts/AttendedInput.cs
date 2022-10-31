using System;
using System.Collections.Generic;
using shape;
using UnityEngine;

public class AttendedInput
{
    private const int TopTarget = 0;
    private const int RightTarget = 1;
    private const int LeftTarget = 2;
    private const int BottomTarget = 3;

    private readonly bool[] _targets = {false, false, false, false};
    private readonly bool[] _pressed = {false, false, false, false};
    
    public float TimeToPress { get; }
    public Shape[] Shapes { get; }

    public AttendedInput(float timeToPress, Shape[] shapes, IReadOnlyCollection<Target> targets)
    {
        if(shapes.Length is < 1 or > 4)
            Debug.LogError("AttendedInput instantiation : shapes length error : " + shapes.Length);
        
        if(targets.Count != shapes.Length)
            Debug.LogError("AttendedInput instantiation : targets length and shapes length should be the same : " + targets.Count + " != " + shapes.Length);
        
        TimeToPress = timeToPress;
        Shapes = shapes;

        foreach (var target in targets)
        {
            switch (target)
            {
                case Target.Top:
                    _targets[TopTarget] = true;
                    break;
                case Target.Right:
                    _targets[RightTarget] = true;
                    break;
                case Target.Left:
                    _targets[LeftTarget] = true;
                    break;
                case Target.Bottom:
                    _targets[BottomTarget] = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void SetPressed(Target target)
    {
        switch (target)
        {
            case Target.Top:
                _pressed[TopTarget] = true;
                break;
            case Target.Right:
                _pressed[RightTarget] = true;
                break;
            case Target.Left:
                _pressed[LeftTarget] = true;
                break;
            case Target.Bottom:
                _pressed[BottomTarget] = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsAllPressed()
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i] && !_pressed[i])
                return false;
        }
        
        return true;
    }
}