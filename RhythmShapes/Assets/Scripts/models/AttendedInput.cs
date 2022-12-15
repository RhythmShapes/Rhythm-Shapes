using System;
using shape;
using Unity.VisualScripting;
using UnityEngine;

namespace models
{
    public class AttendedInput
    {
        public Shape[] Shapes { get; }
        public float TimeToPress { get; }
        public bool MustPressAll { get; }
    
        private const int TopTarget = 0;
        private const int RightTarget = 1;
        private const int LeftTarget = 2;
        private const int BottomTarget = 3;

        private readonly bool[] _pressed = {false, false, false, false};

        public AttendedInput(float timeToPress, Shape[] shapes, bool mustPressAll = false)
        {
            if(shapes.Length is < 1 or > 4)
                Debug.LogError("AttendedInput instantiation : shapes length error : " + shapes.Length);

            TimeToPress = timeToPress;
            Shapes = shapes;
            MustPressAll = mustPressAll;
        }

        public void SetPressed(Target target, bool pressed = true)
        {
            switch (target)
            {
                case Target.Top:
                    _pressed[TopTarget] = pressed;
                    break;
                case Target.Right:
                    _pressed[RightTarget] = pressed;
                    break;
                case Target.Left:
                    _pressed[LeftTarget] = pressed;
                    break;
                case Target.Bottom:
                    _pressed[BottomTarget] = pressed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ResetAllPressed()
        {
            _pressed[TopTarget] = false;
            _pressed[RightTarget] = false;
            _pressed[LeftTarget] = false;
            _pressed[BottomTarget] = false;
        }

        public bool ShouldBePressed(Target target)
        {
            foreach (var shape in Shapes)
            {
                if (shape.Target == target)
                    return true;
            }

            return false;
        }

        public bool AreAllPressed()
        {
            foreach (var shape in Shapes)
            {
                if (shape.Target == Target.Top && !_pressed[TopTarget] ||
                    shape.Target == Target.Right && !_pressed[RightTarget] ||
                    shape.Target == Target.Left && !_pressed[LeftTarget] ||
                    shape.Target == Target.Bottom && !_pressed[BottomTarget])
                    return false;
            }
        
            return true;
        }

        public bool IsPressed(Target target)
        {
            return target switch
            {
                Target.Top => _pressed[TopTarget],
                Target.Right => _pressed[RightTarget],
                Target.Left => _pressed[LeftTarget],
                Target.Bottom => _pressed[BottomTarget],
                _ => throw new IndexOutOfRangeException()
            };
        }
    }
}