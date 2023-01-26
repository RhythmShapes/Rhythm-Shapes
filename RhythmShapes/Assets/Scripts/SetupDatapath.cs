using System.IO;
using UnityEngine;
using utils;

public class SetupDatapath : MonoBehaviour
{
    public bool setupNeeded = true;
    private string applicationPersistentDataPath;
    private void Awake()
    {
        applicationPersistentDataPath = Application.persistentDataPath;

        //if(Directory.Exists(Path.Combine(applicationPersistentDataPath,"Levels","LevelTest")))
        if(Directory.Exists(Path.Combine(applicationPersistentDataPath,"Levels")))
        {
            setupNeeded = false;
        }

        if (setupNeeded)
        {
            Directory.CreateDirectory(Path.Combine(applicationPersistentDataPath, "Levels"));
        }
    }
}
