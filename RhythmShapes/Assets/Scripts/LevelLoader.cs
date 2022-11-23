using System;
using System.Collections;
using System.IO;
using System.Threading;
using AudioAnalysis;
using UnityEditor;
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
        /*if (!File.Exists(audioFilePath))
        {
            Debug.LogError("Cannot find the source audio file : " + audioFilePath);
            return;
        }

        if (!AssetDatabase.IsValidFolder(_resourcesFolderPath + levelName))
        {
            if (AssetDatabase.CreateFolder(_resourcesFolderPath, levelName).Length == 0)
            {
                Debug.LogError("Cannot create new folder : " + _resourcesFolderPath + levelName);
                return;
            }
        }
        
        string audioPath = _resourcesFolderPath + levelName + "/" + audioFileName;

        if (AssetDatabase.FindAssets(audioPath).Length > 0)
        {
            if (AssetDatabase.DeleteAsset(audioPath))
            {
                Debug.LogError("Cannot delete old audio file : " + audioPath);
                return;
            }
        }

        File.Copy(audioFilePath, _resourcesFolderPath + levelName);*/
        
        Debug.Log("0");
        Thread.Sleep(2000);
        _loaded = false;
        analyseAudioSource.clip = LoadAudio(levelName);
        MultiRangeAnalysis.Init(analyseAudioSource.clip);
        onLoadFromAnalysisStart.Invoke();
        Debug.Log("1");
        Thread.Sleep(2000);

        new Thread(() =>
        {
            Debug.Log("2");
            LevelDescription level = MultiRangeAnalysis.AnalyseMusic(_resourcesFolderPath + levelsFolderName + "/" + levelName + "/" + dataFileName + ".xml");
            _currentLevelDescription = level;
            _loaded = true;
            Debug.Log("3");
        }).Start();

        //StartCoroutine(CheckLoadedCo());
    }

    private IEnumerator CheckLoadedCo()
    {
        while (!_loaded)
            yield return null;

        Debug.Log("4");
        onLoadedEvent.Invoke(_currentLevelDescription);
        yield return null;
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