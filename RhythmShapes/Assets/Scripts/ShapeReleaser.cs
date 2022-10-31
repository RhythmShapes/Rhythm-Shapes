using shape;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShapeReleaser : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();

            if (_audioSource.time > input.TimeToPress + model.GoodPressedWindow)
            {
                foreach (var shape in input.Shapes)
                {
                    ShapeFactory.Instance.Release(shape);
                    AccuracyTextManager.Instance.SetAccuracyText(shape.Target, PressedAccuracy.Missed);
                }

                model.PopAttendedInput();
            }
        }
    }
}