using models;
using shape;
using ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using utils;

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
        onInputValidated ??= new UnityEvent<Target, PressedAccuracy>();
        // Debug.Log("InputValidation : GameInfo" + GameInfo.Calibration);
    }

    public void OnInputPerformed(Target target)
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();

            if (input.ShouldBePressed(target))
            {
                // Debug.Log("InputValidation -> OnInputPerformed : " + input.TimeToPress);
                if (SceneManager.GetActiveScene().name == "TestingCalibration")
                {
                    TestingCalibration.Instance.EnqueueShapeTheoricalPressTimeQueue(input.TimeToPress);
                }
                PressedAccuracy accuracy = CalculateAccuracy(input);
                if (accuracy != PressedAccuracy.Missed)
                {
                    if(!input.MustPressAll)
                        onInputValidated.Invoke(target, accuracy);

                    if (input.AreAllPressed())
                    {
                        foreach (var shape in input.Shapes)
                        {
                            ShapeFactory.Instance.Release(shape);
                            if(input.MustPressAll)
                                onInputValidated.Invoke(shape.Target, accuracy);
                        }

                        model.PopAttendedInput();
                    }
                }
            }
        }
    }

    private PressedAccuracy CalculateAccuracy(AttendedInput input)
    {
        GameModel model = GameModel.Instance;

        if (_audioSource.time >= input.TimeToPress - model.PerfectPressedWindow + GameInfo.InputCalibration &&
            _audioSource.time <= input.TimeToPress + model.PerfectPressedWindow + GameInfo.InputCalibration)
        {
            return PressedAccuracy.Perfect;
        }
        
        if (_audioSource.time >= input.TimeToPress - model.GoodPressedWindow + GameInfo.InputCalibration&&
            _audioSource.time <= input.TimeToPress + model.GoodPressedWindow + GameInfo.InputCalibration)
        {
            return PressedAccuracy.Good;
        }
        
        if (_audioSource.time >= input.TimeToPress - model.OkPressedWindow + GameInfo.InputCalibration&&
            _audioSource.time <= input.TimeToPress + model.OkPressedWindow + GameInfo.InputCalibration)
        {
            return PressedAccuracy.Ok;
        }
        
        if (_audioSource.time >= input.TimeToPress - model.BadPressedWindow + GameInfo.InputCalibration&&
            _audioSource.time <= input.TimeToPress + model.BadPressedWindow + GameInfo.InputCalibration)
        {
            return PressedAccuracy.Bad;
        }

        return PressedAccuracy.Missed;
    }
}
