using models;
using shape;
using UnityEngine;
using utils.XML;

namespace edition
{
    public class PathDemo : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private LevelPreparator levelLoader;

        private ShapeModel _current;
        
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

                levelLoader.Init(level);
            }
        }

        public void OnReset()
        {
            audioSource.Stop();
            GameModel model = GameModel.Instance;
            
            while(model.HasNextShapeModel())
                model.PopShapeModel();
            
            while (model.HasNextAttendedInput())
            {
                foreach (var shape in model.GetNextAttendedInput().Shapes)
                    ShapeFactory.Instance.Release(shape);
                model.PopAttendedInput();
            }
        }

        public void OnPreparationDone()
        {
            if (GameModel.Instance.HasNextShapeModel())
            {
                _current = GameModel.Instance.GetNextShapeModel();
                audioSource.Play();
            }
        }

        public void OnShapeArrived()
        {
            audioSource.Stop();
            GameModel.Instance.PushShapeModel(_current);
            audioSource.Play();
        }

        public void OnShapeChanged()
        {
            OnShapeSelected();
        }
    }
}
