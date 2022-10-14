using System.Xml.Serialization;
using UnityEngine;

namespace XML
{
    public class ShapeDescription
    {
        public ShapeType type;
        public float speed;
        public float timeToPress;
        public Vector2[] pathToFollow;
        
        [XmlElement("Color", typeof(Color))]
        public Color color;
    }
}