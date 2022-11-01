using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{
    public class BasicBPM
    {
        public static LevelDescription AnalyseMusic()
        {
            //TODO : REPLACE WITH ANALYSIS CODE
            Debug.LogWarning("TODO : REPLACE WITH ANALYSIS CODE");
            
            string dataPath = "Levels/LevelTest/Data";
        
            TextAsset xml = Resources.Load<TextAsset>(dataPath);
            return  XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        }
    }
}