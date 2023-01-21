using System.IO;
using UnityEngine;

public class SetupDatapath : MonoBehaviour
{
    public bool setupNeeded = true;
    private string applicationPersistentDataPath;
    private string applicationDataPath;
    private void Awake()
    {
        applicationPersistentDataPath = Application.persistentDataPath;
        applicationDataPath = Application.dataPath;
        if(Directory.Exists(Path.Combine(applicationPersistentDataPath,"Levels","LevelTest")))
        {
            setupNeeded = false;
        }

        if (setupNeeded)
        {
            File.Copy(Path.Combine(applicationDataPath,"Resources","Levels","LevelTest"), Path.Combine(applicationPersistentDataPath,"Levels","LevelTest"));
        }
    }
    
    public void SetUpCalibrationStage()
    {
        if(Directory.Exists(Path.Combine(applicationPersistentDataPath,"Levels","Calibration31sec")))
        {
            setupNeeded = false;
        }

        if (setupNeeded)
        {
            File.Copy(Path.Combine(applicationDataPath,"Resources","Levels","Calibration31sec"), Path.Combine(applicationPersistentDataPath,"Levels","Calibration31sec"));
        }
    }
}
