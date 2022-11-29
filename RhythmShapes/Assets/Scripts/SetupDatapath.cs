using System.IO;
using UnityEngine;

public class SetupDatapath : MonoBehaviour
{
    private bool setupNeeded = true;
    private void Awake()
    {
        if(Directory.Exists(Path.Combine(Application.persistentDataPath,"Levels","LevelTest")))
        {
            setupNeeded = false;
        }

        if (setupNeeded)
        {
            File.Copy(Path.Combine(Application.dataPath,"Resources","Levels","LevelTest"), Path.Combine(Application.persistentDataPath,"Levels","LevelTest"));
        }
    }
}
