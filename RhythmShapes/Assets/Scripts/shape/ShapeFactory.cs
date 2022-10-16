using System.Collections.Generic;
using UnityEngine;

namespace shape
{
    public class ShapeFactory : MonoBehaviour
    {
        public static ShapeFactory Instance { get; private set; }
    
        [SerializeField] private GameObject squarePrefab;
    
        private readonly Queue<Shape> _squares = new();

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
                default:
                    if (!_squares.TryDequeue(out shape))
                        shape = Instantiate(squarePrefab).GetComponent<Shape>();
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
                case ShapeType.Square:
                default:
                    _squares.Enqueue(shape);
                    break;
            }
        }
    }
}
