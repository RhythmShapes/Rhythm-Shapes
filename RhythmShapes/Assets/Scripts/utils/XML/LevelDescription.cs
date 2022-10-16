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
        
        [XmlElement("TargetDescription", typeof(TargetDescription))]
        public TargetDescription[] targetColors;

        public Color GetTargetColor(Target target)
        {
            foreach (var targetColor in targetColors)
            {
                if (targetColor.target == target)
                    return targetColor.color;
            }

            // Default but normally unused
            return targetColors[0].color;
        }
    }
}