using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables;
    [SerializeField] private bool loadFromAnalyse;
    [SerializeField] private string devLevelName;
    /*[SerializeField] private string devAudioFilePath;*/
    public string LevelName { get; set; }
    public bool Analyse { get; set; }
    
    /*public string AudioSourceFilePath { get; set; }*/

    private void Start()
    {
        string levelName = useDevVariables ? devLevelName : LevelName;
        
        /*if(!useDevVariables && Analyse)
            LevelLoader.Instance.LoadLevelFromAnalysis(AudioSourceFilePath, levelName);
        else if(useDevVariables && loadFromAnalyse)
            LevelLoader.Instance.LoadLevelFromAnalysis(devAudioFilePath, levelName);*/
        if((!useDevVariables && Analyse) || (useDevVariables && loadFromAnalyse))
            LevelLoader.Instance.LoadLevelFromAnalysis(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
