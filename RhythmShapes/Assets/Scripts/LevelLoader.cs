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
using utils;
using utils.XML;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private AudioSource targetAudioSource;
    [SerializeField] private UnityEvent onLoadFromFileStart;
    [SerializeField] private UnityEvent onLoadFromAnalysisStart;
    [SerializeField] private UnityEvent<LevelDescription> onLoadedEvent;

    private AudioClip _loadedAudioClip;
    private LevelDescription _currentLevelDescription;
    private bool _loaded;

    private void Awake()
    {
        onLoadFromFileStart ??= new UnityEvent();
        onLoadFromAnalysisStart ??= new UnityEvent();
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    public void LoadLevelFromFile(string levelName)
    {
        onLoadFromFileStart.Invoke();

        if (!LevelTools.DoLevelContainsData(levelName))
        {
            Debug.LogError("Cannot find data file : " + LevelTools.GetLevelDataFilePath(levelName));
            return;
        }
        
        LevelDescription level = LevelTools.LoadLevelData(levelName);
        
        StartCoroutine(LoadAudio(LevelTools.GetLevelAudioFilePath(levelName), () =>
        {
            targetAudioSource.clip = _loadedAudioClip;
            _currentLevelDescription = level;
            onLoadedEvent.Invoke(level);
        }, levelName));
    }

    public void LoadLevelFromAnalysis(string sourceAudioPath)
    {
        _loaded = false;
        ProgressUtil.Init(7);
        
        StartCoroutine(LoadAudio(sourceAudioPath, () =>
        {
            targetAudioSource.clip = _loadedAudioClip;
            ProgressUtil.Update();
            MultiRangeAnalysis.Init(_loadedAudioClip);
            onLoadFromAnalysisStart.Invoke();
        
            new Thread(() =>
            {
                LevelDescription level = MultiRangeAnalysis.AnalyseMusic();
                _currentLevelDescription = level;
                _loaded = true;
            }).Start();
        }));
    }

    public void LoadLevelFromRessourcesFolder(string levelName)
    {
        string dataPath =  LevelTools.LevelsFolderName + "/" + levelName + "/" + LevelTools.CompleteDataFileName;
        
        TextAsset xml = Resources.Load<TextAsset>(dataPath);

        if (xml == null)
        {
            Debug.LogError("Cannot find data file : " + dataPath);
            return;
        }
        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(xml);

        targetAudioSource.clip = LoadAudioFromRessourcesFolder(levelName);

        _currentLevelDescription = level;
        onLoadedEvent.Invoke(level);
    }
    // Uses _loadedAudioClip so it need to be not null
    /*public void LaunchAnalysis()
    {
        if (_loadedAudioClip == null)
        {
            Debug.Log("Cannot launch analysis because _loadedAudioClip is null");
            return;
        }

        _loaded = false;
        ProgressUtil.Init(7);
        
        targetAudioSource.clip = _loadedAudioClip;
        ProgressUtil.Update();
        MultiRangeAnalysis.Init(_loadedAudioClip);
        onLoadFromAnalysisStart.Invoke();
        
        new Thread(() =>
        {
            LevelDescription level = MultiRangeAnalysis.AnalyseMusic();
            _currentLevelDescription = level;
            _loaded = true;
        }).Start();
    }*/

    private void FixedUpdate()
    {
        if (!_loaded) return;

        _loaded = false;
        onLoadedEvent.Invoke(_currentLevelDescription);
    }

    public void LoadLevelFromCurrentLevelDescription()
    {
        onLoadedEvent.Invoke(_currentLevelDescription);
    }

    private IEnumerator LoadAudio(string audioFilePath, Action callback, string audioName = "")
    {
        if(File.Exists(audioFilePath))
        {
            using var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.MPEG);
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
                    _loadedAudioClip.name = audioName;
                    callback();
                }
                else Debug.LogError("Couldn't find a valid AudioClip :(");
            }
            else Debug.LogError("The download process is not completely finished.");
        }
        else Debug.LogError("Unable to locate converted song file.");
    }

    private AudioClip LoadAudioFromRessourcesFolder(string levelName)
    {
        string audioPath = LevelTools.LevelsFolderName + "/" + levelName + "/" + LevelTools.CompleteAudioFileName;
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        
        if (audioClip == null)
        {
            Debug.LogError("Cannot find audio file : " + audioPath);
            return null;
        }

        _loadedAudioClip = audioClip;
        
        return audioClip;
    }
}