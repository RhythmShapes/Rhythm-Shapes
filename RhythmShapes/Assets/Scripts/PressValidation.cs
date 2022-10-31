using shape;
using UnityEngine;

public class PressValidation : MonoBehaviour
{
    public static PressValidation Instance { get; private set; }
    
    private AudioSource _audioSource;
    
    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
            
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnInputPerformed()
    {
        GameModel model = GameModel.Instance;
        
        if (model.HasNextAttendedInput())
        {
            AttendedInput input = model.GetNextAttendedInput();

            if (_audioSource.time >= input.TimeToPress - model.GoodPressedWindow &&
                _audioSource.time <= input.TimeToPress + model.GoodPressedWindow && input.IsAllPressed())
            {
                foreach (var shape in input.Shapes)
                {
                    ShapeFactory.Instance.Release(shape);
                    AccuracyTextManager.Instance.SetAccuracyText(shape.Target, PressedAccuracy.Good);
                }

                model.PopAttendedInput();
            }
        }
    }
}
