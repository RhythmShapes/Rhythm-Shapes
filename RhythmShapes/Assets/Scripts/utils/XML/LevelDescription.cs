using System.Xml.Serialization;
using shape;
using UnityEngine;

namespace utils.XML
{
    public class LevelDescription
    {
        [XmlAttribute]
        public string title;
        
        [XmlElement]
        public float audioLength;

        [XmlElement]
        public int numberOfNotes;
        
        [XmlElement]
        public int numberOfDoubleNotes;
        
        [XmlElement]
        public float numberOfNotesPerSecond;
        
        [XmlElement]
        public float minimalNoteDelay;
        
        [XmlElement]
        public float analysisThreshold;
        
        [XmlElement]
        public float doubleNoteAnalysisThreshold;
        
        [XmlElement("ShapeDescription", typeof(ShapeDescription))]
        public ShapeDescription[] shapes;
    }
}