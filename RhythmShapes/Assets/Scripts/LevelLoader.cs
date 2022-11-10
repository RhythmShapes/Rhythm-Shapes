using AudioAnalysis;
using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }
    
    [Header("Levels file paths and names")]
    [Space]
    /* Must be a path in Assets/Resources/ */
    [SerializeField] private string levelsFolderName;
    [SerializeField] private string dataFileName;
    [SerializeField] private string audioFileName;
    [Space]
    [SerializeField] private AudioSource gameAudioSource;
    [SerializeField] private AudioSource analyseAudioSource;
    [SerializeField] private UnityEvent<LevelDescription> onLoadedEvent;
    private LevelDescription _currentLevelDescription;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
        
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    public void LoadLevelFromFile(string levelName)
    {
        string dataPath = levelsFolderName + "/" + levelName + "/" + dataFileName;
        
        TextAsset xml = Resources.Load<TextAsset>(dataPath);

        if (xml == null)
        {
            Debug.LogError("Cannot find data file : " + dataPath);
            return;
        }

        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        gameAudioSource.clip = LoadAudio(levelName);
        _currentLevelDescription = level;
        onLoadedEvent.Invoke(level);
    }

    public void LoadLevelFromAnalyse(string levelName)
    {
        analyseAudioSource.clip = LoadAudio(levelName);
        LevelDescription level = MultiRangeAnalysis.AnalyseMusic(analyseAudioSource.clip,levelName);
        _currentLevelDescription = level;
        onLoadedEvent.Invoke(level);
    }

    public void LoadLevelFromCurrentLevelDescription()
    {
        onLoadedEvent.Invoke(_currentLevelDescription);
    }

    private AudioClip LoadAudio(string levelName)
    {
        string audioPath = levelsFolderName + "/" + levelName + "/" + audioFileName;
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        
        if (audioClip == null)
        {
            Debug.LogError("Cannot find audio file : " + audioPath);
            return null;
        }
        
        return audioClip;
    }
}