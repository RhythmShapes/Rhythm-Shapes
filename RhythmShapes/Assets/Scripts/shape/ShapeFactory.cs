using System.Collections.Generic;
using UnityEngine;

namespace shape
{
    public class ShapeFactory : MonoBehaviour
    {
        public static ShapeFactory Instance { get; private set; }
    
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private GameObject diamondPrefab;
    
        private readonly Queue<Shape> _squares = new();
        private readonly Queue<Shape> _circles = new();
        private readonly Queue<Shape> _diamonds = new();

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        public Shape GetShape(ShapeType type)
        {
            Shape shape;

            switch (type)
            {
                case ShapeType.Square:
                    if (!_squares.TryDequeue(out shape))
                        shape = Instantiate(squarePrefab,gameObject.transform).GetComponent<Shape>();
                    break;
                
                case ShapeType.Circle:
                    if (!_circles.TryDequeue(out shape))
                        shape = Instantiate(circlePrefab,gameObject.transform).GetComponent<Shape>();
                    break;
                
                case ShapeType.Diamond:
                    if (!_diamonds.TryDequeue(out shape))
                        shape = Instantiate(diamondPrefab,gameObject.transform).GetComponent<Shape>();
                    break;
                
                default:
                    Debug.LogError("Unknown ShapeType, using Square");
                    if (!_squares.TryDequeue(out shape))
                        shape = Instantiate(squarePrefab,gameObject.transform).GetComponent<Shape>();
                    break;
            }

            shape.gameObject.SetActive(true);
            return shape;
        }

        public void Release(Shape shape)
        {
            shape.gameObject.SetActive(false);
        
            switch (shape.Type)
            {
                case ShapeType.Circle:
                    _circles.Enqueue(shape);
                    break;
                
                case ShapeType.Diamond:
                    _diamonds.Enqueue(shape);
                    break;
                
                case ShapeType.Square:
                default:
                    _squares.Enqueue(shape);
                    break;
            }
        }

        public void ReleaseAll()
        {
            var allShapes = GetComponentsInChildren<Shape>();
            foreach (var shape in allShapes)
            {
                if (shape.gameObject.activeSelf)
                {
                    shape.gameObject.SetActive(false);
        
                    switch (shape.Type)
                    {
                        case ShapeType.Circle:
                            _circles.Enqueue(shape);
                            break;
                
                        case ShapeType.Diamond:
                            _diamonds.Enqueue(shape);
                            break;
                
                        case ShapeType.Square:
                        default:
                            _squares.Enqueue(shape);
                            break;
                    }
                }
                
            }

        }
    }
}
