using models;
using shape;
using ui;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ShapeReleaser : MonoBehaviour
{
    [SerializeField] private UnityEvent<Target, PressedAccuracy> onInputMissed;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        onInputMissed ??= new UnityEvent<Target, PressedAccuracy>();
    }
    
    private void Update()
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();

            if (_audioSource.time > input.TimeToPress + model.BadPressedWindow + TestingCalibration.Instance.calibration)
            {
                foreach (var shape in input.Shapes)
                {
                    ShapeFactory.Instance.Release(shape);
                    
                    if(!input.IsPressed(shape.Target) || input.MustPressAll)
                        onInputMissed.Invoke(shape.Target, PressedAccuracy.Missed);
                }

                model.PopAttendedInput();
            }
        }
    }
}