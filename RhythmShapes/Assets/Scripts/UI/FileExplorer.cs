using System;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using UnityEditor;
using UnityEngine;

namespace UI
{
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
        }

        public void OpenExplorer()
        {
            // https://github.com/yasirkula/UnitySimpleFileBrowser
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Musics", ".mp3"));
            FileBrowser.SetDefaultFilter(".mp3");
            StartCoroutine(ShowLoadDialogCoroutine());
        }

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
                    File.Copy(selectedAudioPath, levelPath+"/Audio"+audioExtension);
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
            }
        
        }
        
        // https://github.com/yasirkula/UnitySimpleFileBrowser
        private IEnumerator ShowLoadDialogCoroutine()
        {
            // Show a load file dialog and wait for a response from user
            // Load file/folder: both, Allow multiple selection: true
            // Initial path: default (Documents), Initial filename: empty
            // Title: "Load File", Submit button text: "Load"
            yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load" );

            // Dialog is closed
            // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
            Debug.Log(FileBrowser.Success);

            if( FileBrowser.Success )
            {
                // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
                for(int i = 0; i < FileBrowser.Result.Length; i++)
                    Debug.Log(FileBrowser.Result[i]);
                
                if (FileBrowser.Result.Length > 0 && FileBrowser.Result[0] != null && FileBrowser.Result[0].Length > 0)
                {
                    selectedAudioPath = FileBrowser.Result[0];
                    var qqch = Path.GetFileName(selectedAudioPath);
                    GameInfo.LevelName = qqch.Split(".")[0];
                    audioExtension = Path.GetExtension(selectedAudioPath);
                    Debug.Log(GameInfo.LevelName + ", "+audioExtension);
                    UpdateAudioClip();
                }
            }
        }
    }
}
