﻿using System;
using edition.timeLine;
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
        public static bool HasShapeBeenModified { get; set; }

        public static void Reset()
        {
            OriginLevel = null;
            AnalyzedLevel = null;
            HasShapeBeenModified = false;
        }

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
            MusicPath = string.Empty;
            AnalyzedMusicPath = string.Empty;
            UseLevelMusic = true;
            Shape = null;
            HasShapeBeenModified = false;
        }
        
        public void SetShape(EditorShape shape)
        {
            Shape = shape;
        }

        public static bool HasLevelSet()
        {
            return OriginLevel != null || AnalyzedLevel != null;
        }

        public static LevelDescription GetCurrentLevel()
        {
            return HasBeenAnalyzed() ? AnalyzedLevel : OriginLevel;
        }

        public static bool HasBeenAnalyzed()
        {
            return AnalyzedLevel != null;
        }

        public static bool IsInspectingShape()
        {
            return Shape != null;
        }

        public void ResetInspectedShape()
        {
            Shape = null;
        }
    }
}