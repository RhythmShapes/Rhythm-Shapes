
using UnityEngine;

namespace shape
{
    public class ShapeModel
    {
        public ShapeType Type { get; }
        public Target Target { get; }
        public Color Color { get; }
        public Vector2[] PathToFollow { get; }
        public float TimeToPress { get; set; }
        public float TimeToSpawn { get; }
        public float Speed { get; }

        public ShapeModel(ShapeType type, Target target, Color color, Vector2[] pathToFollow, float timeToPress, float timeToSpawn, float speed)
        {
            Type = type;
            Target = target;
            Color = color;
            PathToFollow = pathToFollow;
            TimeToPress = timeToPress;
            TimeToSpawn = timeToSpawn;
            Speed = speed;
        }
    }
}