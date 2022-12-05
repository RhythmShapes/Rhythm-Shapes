using System;
using UnityEngine;
using utils.XML;

namespace edition
{
    [Obsolete]
    public class EditorModel : MonoBehaviour
    {
        public static LevelDescription Level;
        public static ShapeDescription Shape;
        public static string LevelName;
        public static string SourceAudioPath;

        public void SetLevel(LevelDescription level)
        {
            Level = level;
        }

        public void SetShape(ShapeDescription shape)
        {
            Shape = shape;
        }

        public static bool IsLevelDefined()
        {
            return Level != null;
        }

        public static bool IsShapeDefined()
        {
            return Shape != null;
        }

        public static bool IsLevelNameDefined()
        {
            return !string.IsNullOrEmpty(LevelName);
        }

        public static bool IsSourceAudioPathDefined()
        {
            return !string.IsNullOrEmpty(SourceAudioPath);
        }
    }
}