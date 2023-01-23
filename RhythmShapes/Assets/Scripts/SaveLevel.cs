using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;
using utils.XML;

public class SaveLevel : MonoBehaviour
{
    
    public void SaveLevelAfterAnalysis(LevelDescription levelData)
    {
        if (GameInfo.IsNewLevel)
        {
            GameInfo.IsNewLevel = false;
            LevelTools.CreateLevelFolder(GameInfo.LevelName);
            LevelTools.SaveLevelAudio(GameInfo.LevelName, PresetDifficulty.Instance.musicPath);
            LevelTools.SaveLevelData(GameInfo.LevelName, levelData);
        }
    }
}
    
