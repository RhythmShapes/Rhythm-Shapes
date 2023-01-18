using System;
using models;
using shape;
using UnityEngine;
using utils.XML;

namespace edition
{
    public class PathDemo : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private ShapeModel _current;
        private bool _doAct;

        private void Update()
        {
            if(!_doAct)
                return;
            
            GameModel model = GameModel.Instance;
            
            if (model.HasNextShapeModel())
            {
                ShapeModel shapeModel = model.GetNextShapeModel();

                if (shapeModel.TimeToSpawn <= audioSource.time)
                {
                    Shape shape = ShapeFactory.Instance.GetShape(shapeModel.Type);
                    shape.Init(shapeModel);
                    model.PushAttendedInput(new AttendedInput(shape.TimeToPress, new[] { shape }));
                    model.PopShapeModel();
                }
            }

            if (!model.HasNextAttendedInput()) return;

            AttendedInput input = model.GetNextAttendedInput();

            if (!(audioSource.time > input.TimeToPress + model.GoodPressedWindow)) return;

            foreach (var shape in input.Shapes)
            {
                ShapeFactory.Instance.Release(shape);
            
                if(!input.IsPressed(shape.Target) || input.MustPressAll)
                    OnShapeArrived();
            }

            model.PopAttendedInput();
        }

        public void OnShapeSelected()
        {
            if (EditorModel.IsInspectingShape())
            {
                OnReset();

                ShapeDescription shapeDescription = EditorModel.Shape.Description;
                ShapeDescription copy = new ShapeDescription()
                {
                    goRight = shapeDescription.goRight,
                    target = shapeDescription.target,
                    timeToPress = 1f,
                    type = shapeDescription.type
                };
                
                LevelDescription level = new LevelDescription
                {
                    title = "PathDemo",
                    shapes = new[] { copy }
                };

                _doAct = true;
                FindObjectOfType<LevelPreparator>().Init(level);
            }
        }

        public void OnReset()
        {
            audioSource.Stop();
            GameModel.Instance.Reset();
        }

        public void OnPreparationDone()
        {
            if (_doAct && GameModel.Instance.HasNextShapeModel())
            {
                _current = GameModel.Instance.GetNextShapeModel();
                audioSource.Play();
            }
        }

        public void OnShapeArrived()
        {
            if (_doAct)
            {
                audioSource.Stop();
                GameModel.Instance.PushShapeModel(_current);
                audioSource.Play();
            }
        }

        public void OnShapeChanged()
        {
            OnShapeSelected();
        }

        public void DoAct(bool act)
        {
            _doAct = act;
        }
    }
}
