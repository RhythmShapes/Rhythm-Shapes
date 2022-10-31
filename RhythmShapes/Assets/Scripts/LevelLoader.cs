using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class LevelLoader : MonoBehaviour
{
    public UnityEvent<LevelDescription> onLoadedEvent;
    
    [Header("Test values")]
    [Space]
    [SerializeField] private TextAsset levelXML;

    private void Awake()
    {
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    private void Start()
    {
        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(levelXML);
        onLoadedEvent.Invoke(level);
    }
}