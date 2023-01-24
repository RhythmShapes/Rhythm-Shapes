using System;
using System.Collections.Generic;
using models;
using shape;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShapeSpawner : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    private void Update()
    {
        GameModel model = GameModel.Instance;
        List<Shape> shapes = new List<Shape>();

        while (model.HasNextShapeModel())
        {
            ShapeModel shapeModel = model.GetNextShapeModel();
            
            if (shapeModel.TimeToSpawn + GameInfo.AudioCalibration <= _audioPlayer.time)
            {
                if (shapes.Count > 0 && Math.Abs(shapes[0].TimeToPress - shapeModel.TimeToPress) != 0f)
                {
                    model.PushAttendedInput(new AttendedInput(shapes[0].TimeToPress + GameInfo.AudioCalibration, shapes.ToArray()));
                    shapes.Clear();
                }
                
                Shape shape = ShapeFactory.Instance.GetShape(shapeModel.Type);
                shape.Init(shapeModel);
                
                shapes.Add(shape);
                model.PopShapeModel();
            }
            else break;
        }

        if (shapes.Count > 0)
        {
            if (shapes.Count > 1)
            {
                foreach (var shape in shapes)
                {
                    shape.ShowOutline();
                }
                model.PushAttendedInput(new AttendedInput(shapes[0].TimeToPress + GameInfo.AudioCalibration, shapes.ToArray(),true));
            }
            else
            {
                model.PushAttendedInput(new AttendedInput(shapes[0].TimeToPress + GameInfo.AudioCalibration, shapes.ToArray()));
            }
        }
    }
}