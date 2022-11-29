using System.Xml.Serialization;
using shape;
using UnityEngine;

namespace utils.XML
{
    public class LevelDescription
    {
        [XmlAttribute]
        public string title;
        
        [XmlElement("ShapeDescription", typeof(ShapeDescription))]
        public ShapeDescription[] shapes;
    }
}