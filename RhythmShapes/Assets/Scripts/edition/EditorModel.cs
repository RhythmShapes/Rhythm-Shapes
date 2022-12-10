using System;
using UnityEngine;
using utils.XML;

namespace edition
{
    public class EditorModel : MonoBehaviour
    {
        public static LevelDescription OriginLevel { get; private set; }
        public static LevelDescription AnalyzedLevel { get; private set; }
        public static string MusicPath { get; set; } = string.Empty;
        public static string AnalyzedMusicPath { get; set; } = string.Empty;
        public static string LevelName { get; set; } = string.Empty;
        public static bool UseLevelMusic { get; set; }
        public static EditorShape Shape { get; set; }

        public void SetLevel(LevelDescription level)
        {
            if (!GameInfo.IsNewLevel && OriginLevel == null)
                OriginLevel = level;
            else
                AnalyzedLevel = level;
        }

        public void SetAfterSave(LevelDescription level)
        {
            GameInfo.IsNewLevel = false;
            OriginLevel = level;
            AnalyzedLevel = null;
            MusicPath = String.Empty;
            AnalyzedMusicPath = String.Empty;
            LevelName = String.Empty;
            UseLevelMusic = true;
            Shape = null;
        }
        
        public void SetShape(EditorShape shape)
        {
            Shape = shape;
        }

        public static bool HasBeenAnalyzed()
        {
            return AnalyzedLevel != null;
        }

        public static bool IsInspectingShape()
        {
            return Shape != null;
        }
    }
}