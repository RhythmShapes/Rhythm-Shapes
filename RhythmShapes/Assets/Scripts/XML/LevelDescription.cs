using System.Xml.Serialization;

namespace XML
{
    public class LevelDescription
    {
        [XmlAttribute]
        public string title;
        
        [XmlElement("ShapeDescription", typeof(ShapeDescription))]
        public ShapeDescription[] shapes;
    }
}