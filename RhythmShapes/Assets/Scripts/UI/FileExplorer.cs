using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using File = UnityEngine.Windows.File;

public class FileExplorer : MonoBehaviour
{
    public static FileExplorer Instance { get; private set; }
    public string selectedAudioPath;
    public AudioClip audioClip;
    private string applicationDataPath;
    // private string _resourcesFolderPath = "Assets/Resources/";
    private string levelPath;
    // public string GameInfo.LevelName = "LevelTest";
    public string audioExtension;

    public void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        GameInfo.LevelName = "LevelTest";
        applicationDataPath = Application.persistentDataPath;
        Debug.Log(applicationDataPath);//Application.dataPath;
        levelPath = applicationDataPath + "/Levels";
        DontDestroyOnLoad(gameObject);
    }

    public void OpenExplorer()
    {
        selectedAudioPath = EditorUtility.OpenFilePanel("Select your music", "", "mp3");
        if (selectedAudioPath != null && selectedAudioPath.Length>0)
        {
            var qqch = Path.GetFileName(selectedAudioPath);
            GameInfo.LevelName = qqch.Split(".")[0];
            audioExtension = Path.GetExtension(selectedAudioPath);
            Debug.Log(GameInfo.LevelName + ", "+audioExtension);
            UpdateAudioClip();
        }
        
    }

    // public void CreateFolder()
    // {
    //     // Debug.Log(Application.dataPath);
    //     
    //     if (!Directory.Exists(levelPath))
    //     {
    //         Debug.Log("kebab");
    //         
    //         // AssetDatabase.CreateFolder("Assets/Resources/", GameInfo.LevelName); not working
    //     }
    // }

    private void UpdateAudioClip() 
    {
        if (!File.Exists(selectedAudioPath))
        {
            Debug.LogError("Cannot find the source audio file : " + selectedAudioPath);
            return;
        }

        levelPath += "/"+ GameInfo.LevelName;
        Debug.Log(levelPath);
        
            if (!Directory.Exists(levelPath))
            {
                Directory.CreateDirectory(levelPath);
                try
                {
                    GameInfo.requestAnalysis = true;
                    FileUtil.CopyFileOrDirectory(selectedAudioPath, levelPath+"/Audio"+audioExtension);
                    SceneTransition.Instance.LoadScene(1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                Debug.LogError("level"+ Path.GetDirectoryName(levelPath) +" exist already");
                return;
            }
        
    }
    
    

    // if (!File.Exists(audioFilePath))
    // {
    //     Debug.LogError("Cannot find the source audio file : " + audioFilePath);
    //     return;
    // }
    //     
    //     
    // if (!AssetDatabase.IsValidFolder(_resourcesFolderPath + GameInfo.LevelName))
    // {
    //     if (AssetDatabase.CreateFolder(_resourcesFolderPath, GameInfo.LevelName).Length == 0)
    //     {
    //         Debug.LogError("Cannot create new folder : " + _resourcesFolderPath + GameInfo.LevelName);
    //         return;
    //     }
    // }
    //     
    // string audioPath = _resourcesFolderPath + GameInfo.LevelName + "/" + audioFileName;
    //
    // if (AssetDatabase.FindAssets(audioPath).Length > 0)
    // {
    //     if (AssetDatabase.DeleteAsset(audioPath))
    //     {
    //         Debug.LogError("Cannot delete old audio file : " + audioPath);
    //         return;
    //     }
    // }
    //
    // File.Copy(audioFilePath, _resourcesFolderPath + GameInfo.LevelName);
}
