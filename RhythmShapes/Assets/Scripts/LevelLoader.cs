using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class LevelLoader : MonoBehaviour
{
    public UnityEvent<LevelDescription> onLoadedEvent;
    
    [Header("Tests")]
    [Space]
    [SerializeField] private TextAsset defaultXML;

    private void Awake()
    {
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    private void Start()
    {
        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(defaultXML);
        onLoadedEvent.Invoke(level);
    }
}