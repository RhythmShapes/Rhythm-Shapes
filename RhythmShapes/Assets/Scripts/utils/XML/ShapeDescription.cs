using shape;
using UnityEngine;

namespace utils.XML
{
    public class ShapeDescription
    {
        public ShapeType type;
        public Target target;
        public Vector2[] pathToFollow;
        public float speed;
        public float timeToPress;
        public bool goRight;
    }
}