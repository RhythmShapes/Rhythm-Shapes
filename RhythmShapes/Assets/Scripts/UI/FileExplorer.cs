using System.Collections;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using utils;

namespace UI
{
    public class FileExplorer : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onAudioSelected;
        public static FileExplorer Instance { get; private set; }
        
        private string _selectedAudioPath;
        private bool _audioSelected = false;

        public void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;

            GameInfo.LevelName = "LevelTest";
        }

        private void FixedUpdate()
        {
            if(!_audioSelected) return;

            _audioSelected = false;
            onAudioSelected.Invoke(_selectedAudioPath);
        }

        public void OpenExplorer()
        {
            // https://github.com/yasirkula/UnitySimpleFileBrowser
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Musics", LevelTools.AudioFileExtension));
            FileBrowser.SetDefaultFilter(LevelTools.AudioFileExtension);
            StartCoroutine(ShowLoadDialogCoroutine());
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
            //Debug.Log(FileBrowser.Success);

            if( FileBrowser.Success )
            {
                // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
                /*for(int i = 0; i < FileBrowser.Result.Length; i++)
                    Debug.Log(FileBrowser.Result[i]);*/
                
                if (FileBrowser.Result.Length > 0 && FileBrowser.Result[0] != null && FileBrowser.Result[0].Length > 0)
                {
                    _selectedAudioPath = FileBrowser.Result[0];
                    _audioSelected = true;
                }
            }
        }
    }
}
