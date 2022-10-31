using shape;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShapeSpawner : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        GameModel model = GameModel.Instance;

        while (model.HasNextShapeModel())
        {
            ShapeModel shapeModel = model.GetNextShapeModel();

            if (shapeModel.TimeToSpawn <= _audioSource.time)
            {
                Shape[] shape = new Shape[1];
                shape[0] = ShapeFactory.Instance.GetShape(shapeModel.Type);
                shape[0].Init(shapeModel);

                Target[] target = new Target[1];
                target[0] = shapeModel.Target;

                model.AddAttendedInput(new AttendedInput(shapeModel.TimeToPress, shape, target));
                model.PopShapeDescription();
            }
            else break;
        }
    }
}