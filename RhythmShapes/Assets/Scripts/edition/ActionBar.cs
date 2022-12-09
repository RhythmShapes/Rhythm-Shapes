using UnityEngine;
using UnityEngine.Events;
using utils;
using utils.XML;

namespace edition
{
    public class ActionBar : MonoBehaviour
    {
        [SerializeField] private PopupWindow popupWindow;
        [SerializeField] private UnityEvent<LevelDescription> onSaved;

        private bool _hasStartAnalysisDuringSave = false;

        private void Awake()
        {
            onSaved ??= new UnityEvent<LevelDescription>();
        }

        public void OnQuit()
        {
            SceneTransition.Instance.BackToMainMenu();
        }

        public void OnSave()
        {
            string musicPath = EditorModel.MusicPath;
            string levelName = EditorModel.LevelName;
            
            // Verify fields value
            if (!EditorPanel.CheckFields())
            {
                popupWindow.ShowError("An error prevents saving. Please fix it and try again.");
                return;
            }
            
            // Nothing to save
            if (!EditorModel.HasBeenAnalyzed() && !GameInfo.IsNewLevel && EditorModel.UseLevelMusic && levelName.Equals(EditorModel.OriginLevel.title))
            {
                popupWindow.ShowInfo("Nothing to save.");
                return;
            }

            // Music path changed without being analyzed after
            // All values checks must be done before
            if ((GameInfo.IsNewLevel || !EditorModel.UseLevelMusic) && !EditorModel.AnalyzedMusicPath.Equals(musicPath))
            {
                popupWindow.ShowQuestion(
                    "The music path has changed and has not been analyzed since. Do you want to analyse it now to continue saving ?",
                    callback: confirm =>
                    {
                        if(!confirm)return;
                        
                        _hasStartAnalysisDuringSave = true;
                        EditorPanel.RequestAnalysis();
                    });

                return;
            }
            
            _hasStartAnalysisDuringSave = true;
            ContinueSaving();
        }

        public void ContinueSaving()
        {
            if(!_hasStartAnalysisDuringSave) return;
            _hasStartAnalysisDuringSave = false;
            
            string musicPath = EditorModel.MusicPath;
            string levelName = EditorModel.LevelName;
            
            if (GameInfo.IsNewLevel)
            {
                // New level
                Debug.Log("NEW");
                LevelTools.CreateLevelFolder(levelName);
                LevelTools.SaveLevelAudio(levelName, musicPath);
                LevelTools.SaveLevelData(levelName, EditorModel.AnalyzedLevel);
                onSaved.Invoke(EditorModel.AnalyzedLevel);
                return;
            }

            LevelDescription newOriginLevel = EditorModel.OriginLevel;
            if(!levelName.Equals(EditorModel.OriginLevel.title))
            {
                // Rename level
                string oldLevelName = EditorModel.OriginLevel.title;
                LevelTools.CreateLevelFolder(levelName);
                LevelTools.CopyLevelAudio(oldLevelName, levelName);
                LevelTools.SaveLevelData(levelName, EditorModel.OriginLevel);
                LevelTools.DeleteLevelFolder(oldLevelName);
            }

            if (!string.IsNullOrEmpty(musicPath))
            { 
                // Change level audio
                Debug.Log("CHANGE AUDIO");
                LevelTools.SaveLevelAudio(levelName, musicPath);
            }

            if (EditorModel.HasBeenAnalyzed())
            { 
                // Change level data
                Debug.Log("CHANGE DATA");
                LevelTools.SaveLevelData(levelName, EditorModel.AnalyzedLevel);
                newOriginLevel = EditorModel.AnalyzedLevel;
            }

            onSaved.Invoke(newOriginLevel);
        }

        public void OnTest()
        {
            
        }

        private void ShowPopupError(string message)
        {
            if(popupWindow != null)
                popupWindow.ShowError(message, "Save error");
        }
    }
}