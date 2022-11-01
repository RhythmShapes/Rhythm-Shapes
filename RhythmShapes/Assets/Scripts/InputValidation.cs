using models;
using shape;
using ui;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class InputValidation : MonoBehaviour
{
    public static InputValidation Instance { get; private set; }

    [SerializeField] private UnityEvent<Target, PressedAccuracy> onInputValidated;
    
    private AudioSource _audioSource;
    
    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
            
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnInputPerformed(Target target)
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();
            
            if(!input.ShouldBePressed(target))
                return;

            if (_audioSource.time >= input.TimeToPress - model.GoodPressedWindow &&
                _audioSource.time <= input.TimeToPress + model.GoodPressedWindow)
            {
                if(!input.MustPressAll)
                    onInputValidated?.Invoke(target, PressedAccuracy.Good);
                
                if (!input.AreAllPressed())
                    return;
                
                foreach (var shape in input.Shapes)
                {
                    ShapeFactory.Instance.Release(shape);
                    if(input.MustPressAll)
                        onInputValidated?.Invoke(shape.Target, PressedAccuracy.Good);
                }

                model.PopAttendedInput();
            }
        }
    }
}
