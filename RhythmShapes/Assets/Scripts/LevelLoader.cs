using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AudioAnalysis;
using ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
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
    [Space]
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useBasicBPM;

    private AudioClip _loadedAudioClip;
    private LevelDescription _currentLevelDescription;
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
        string dataPath = GetDataFilePath(levelName);

        TextAsset xml = new TextAsset(File.ReadAllText(dataPath));
        if (xml == null)
        {
            Debug.LogError("Cannot load data file : " + dataPath);
            return;
        }

        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        StartCoroutine(LoadAudio(levelName, () =>
        {
            gameAudioSource.clip = _loadedAudioClip;
            _currentLevelDescription = level;
            onLoadedEvent.Invoke(level);
        }));
    }

    public void LoadLevelFromAnalysis(string levelName)
    {
        _loaded = false;
        AnalyseSlider.Progress.Init(useBasicBPM ? 6 : 8);
        StartCoroutine(LoadAudio(levelName, () =>
        {
            AnalyseSlider.Progress.Update();
            analyseAudioSource.clip = _loadedAudioClip;
            
            if(useBasicBPM) BasicBPM.Init(_loadedAudioClip);
            else MultiRangeAnalysis.Init(_loadedAudioClip);
            
            onLoadFromAnalysisStart.Invoke();

            string dataPath = GetDataFilePath(levelName);
            new Thread(() =>
            {
                LevelDescription level = useBasicBPM ? BasicBPM.AnalyseMusic(dataPath) : MultiRangeAnalysis.AnalyseMusic(dataPath);
                _currentLevelDescription = level;
                _loaded = true;
            }).Start();
        }));
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

    private IEnumerator LoadAudio(string levelName, Action callback)
    {
        string songPath = GetAudioFilePath(levelName);
        if(File.Exists(songPath))
        {
            using var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + songPath, AudioType.MPEG);
            ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = false;
 
            yield return uwr.SendWebRequest();
 
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }
 
            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
 
            if (dlHandler.isDone)
            {
                AudioClip audioClip = dlHandler.audioClip;

                if (audioClip != null)
                {
                    _loadedAudioClip = DownloadHandlerAudioClip.GetContent(uwr);
                    _loadedAudioClip.name = levelName;
                    callback();
                }
                else Debug.LogError("Couldn't find a valid AudioClip :(");
            }
            else Debug.LogError("The download process is not completely finished.");
        }
        else Debug.LogError("Unable to locate converted song file.");
    }

    private string GetLevelDirectoryPath(string levelName)
    {
        return Path.Combine(Application.persistentDataPath, "Levels/", levelName);
    }

    private string GetDataFilePath(string levelName)
    {
        return Path.Combine(GetLevelDirectoryPath(levelName), dataFileName + ".xml");
    }

    private string GetAudioFilePath(string levelName)
    {
        return Path.Combine(GetLevelDirectoryPath(levelName), audioFileName + ".mp3");
    }
}