using UnityEngine;
using XML;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextAsset levelXML;

    private LevelDescription _level;
    
    private void Start()
    {
        _level = XmlHelpers.DeserializeDatabaseFromXML<LevelDescription>(levelXML).ToArray()[0];
        
        Debug.Log(_level.title);
        foreach (var s in _level.shapes)
        {
            Debug.Log(s.color+" "+s.type+" "+s.timeToPress+" "+s.speed);
        }
    }
}