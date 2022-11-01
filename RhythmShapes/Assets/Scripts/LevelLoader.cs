using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }
    
    public UnityEvent<LevelDescription> onLoadedEvent;
    
    [Header("Tests")]
    [Space]
    [SerializeField] private TextAsset defaultXML;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    private void Start()
    {
        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(defaultXML);
        onLoadedEvent.Invoke(level);
    }
}