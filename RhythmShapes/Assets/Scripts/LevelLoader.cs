using System.Threading;
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
    [SerializeField] private UnityEvent onLoadFromFileStart;
    [SerializeField] private UnityEvent onLoadFromAnalysisStart;
    [SerializeField] private UnityEvent<LevelDescription> onLoadedEvent;
    
    private LevelDescription _currentLevelDescription;
    private string _resourcesFolderPath = "Assets/Resources/";
    private bool _loaded;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
        
        onLoadFromFileStart ??= new UnityEvent();
        onLoadFromAnalysisStart ??= new UnityEvent();
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    public void LoadLevelFromFile(string levelName)
    {
        onLoadFromFileStart.Invoke();
        string dataPath = GetDataPath(levelName);
        
        TextAsset xml = Resources.Load<TextAsset>(dataPath);

        if (xml == null)
        {
            Debug.LogError("Cannot load data file : " + dataPath);
            return;
        }

        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        gameAudioSource.clip = LoadAudio(levelName);
        
        _currentLevelDescription = level;
        onLoadedEvent.Invoke(level);
    }

    public void LoadLevelFromAnalysis(string levelName)
    {
        _loaded = false;
        analyseAudioSource.clip = LoadAudio(levelName);
        MultiRangeAnalysis.Init(analyseAudioSource.clip);
        onLoadFromAnalysisStart.Invoke();

        new Thread(() =>
        {
            LevelDescription level = MultiRangeAnalysis.AnalyseMusic(_resourcesFolderPath + levelsFolderName + "/" + levelName + "/" + dataFileName + ".xml");
            _currentLevelDescription = level;
            _loaded = true;
        }).Start();
    }

    private void Update()
    {
        if (!_loaded) return;

        _loaded = false;
        onLoadedEvent.Invoke(_currentLevelDescription);
    }

    public void LoadLevelFromCurrentLevelDescription()
    {
        onLoadedEvent.Invoke(_currentLevelDescription);
    }

    private AudioClip LoadAudio(string levelName)
    {
        string audioPath = GetAudioPath(levelName);
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        
        if (audioClip == null)
        {
            Debug.LogError("Cannot load audio file : " + audioPath);
            return null;
        }
        
        return audioClip;
    }

    private string GetDataPath(string levelName)
    {
        return levelsFolderName + "/" + levelName + "/" + dataFileName;
    }

    private string GetAudioPath(string levelName)
    {
        return levelsFolderName + "/" + levelName + "/" + audioFileName;
    }
}