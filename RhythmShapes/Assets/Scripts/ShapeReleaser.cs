using models;
using shape;
using ui;
using UnityEngine;
using UnityEngine.Events;
using utils;

[RequireComponent(typeof(AudioSource))]
public class ShapeReleaser : MonoBehaviour
{
    [SerializeField] private UnityEvent<Target, PressedAccuracy> onInputMissed;
    
    private AudioSource _audioSource;
    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioPlayer = GetComponent<AudioPlayer>();
        onInputMissed ??= new UnityEvent<Target, PressedAccuracy>();
    }
    
    private void Update()
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();

            if (_audioPlayer.time > input.TimeToPress + GameInfo.AudioCalibration + model.BadPressedWindow + GameInfo.InputCalibration)
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