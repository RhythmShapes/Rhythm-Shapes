using shape;
using UnityEngine;

namespace utils.XML
{
    public class ShapeDescription
    {
        public ShapeType type;
        public Target target;
        public float timeToPress;
        public bool goRight;

        public bool IsEqualTo(ShapeDescription compare)
        {
            return (type == compare.type
                    && target == compare.target
                    && timeToPress.Equals(compare.timeToPress)
                    && goRight == compare.goRight);
        }
    }
}