﻿using System.IO;
using UnityEngine;
using utils.XML;

namespace utils
{
    public static class LevelTools
    {
        public const string LevelsFolderName = "Levels";
        public const string AudioFileName = "Audio";
        public const string AudioFileExtension = ".mp3";
        public const string CompleteAudioFileName = AudioFileName + AudioFileExtension;
        public const string DataFileName = "Data";
        public const string DataFileExtension = ".xml";
        public const string CompleteDataFileName = DataFileName + DataFileExtension;
            
        public static bool DoLevelExists(string levelName)
        {
            return Directory.Exists(GetLevelFolderPath(levelName));
        }
            
        public static bool DoLevelContainsAudio(string levelName)
        {
            return File.Exists(GetLevelAudioFilePath(levelName));
        }
            
        public static bool DoLevelContainsData(string levelName)
        {
            return File.Exists(GetLevelDataFilePath(levelName));
        }
            
        public static bool DoLevelContainsAll(string levelName)
        {
            return DoLevelContainsAudio(levelName) && DoLevelContainsData(levelName);
        }

        public static bool IsFilePathValid(string path)
        {
            return File.Exists(path);
        }

        public static string GetAllLevelsFolderPath()
        {
            return Path.Combine(Application.persistentDataPath, LevelsFolderName);
        }

        public static string GetLevelFolderPath(string levelName)
        {
            return Path.Combine(Application.persistentDataPath, LevelsFolderName, levelName);
        }

        public static string GetLevelAudioFilePath(string levelName)
        {
            return Path.Combine(GetLevelFolderPath(levelName), CompleteAudioFileName);
        }

        public static string GetLevelDataFilePath(string levelName)
        {
            return Path.Combine(GetLevelFolderPath(levelName), CompleteDataFileName);
        }

        public static void CreateLevelFolder(string levelName)
        {
            Directory.CreateDirectory(GetLevelFolderPath(levelName));
        }

        public static void DeleteLevelFolder(string levelName)
        {
            Directory.Delete(GetLevelFolderPath(levelName), true);
        }

        public static void CopyLevelAudio(string oldLevelName, string newLevelName)
        {
            SaveLevelAudio(newLevelName, GetLevelAudioFilePath(oldLevelName));
        }

        public static void SaveLevelAudio(string levelName, string sourcePath)
        {
            string old = GetLevelAudioFilePath(levelName);
            if(File.Exists(old)) File.Delete(old);
            File.Copy(sourcePath, old);
        }

        public static void SaveLevelData(string levelName, LevelDescription level)
        {
            level.title = levelName;
            XmlHelpers.SerializeToXML<LevelDescription>(GetLevelDataFilePath(levelName), level);
        }

        public static LevelDescription LoadLevelData(string levelName)
        {
            TextAsset xml = new TextAsset(File.ReadAllText(GetLevelDataFilePath(levelName)));
            return XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        }
    }
}