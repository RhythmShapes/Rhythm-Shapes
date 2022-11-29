using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables;
    [SerializeField] private bool loadFromAnalyse;
    [SerializeField] private string devLevelName;

    private void Start()
    {
        string levelName = useDevVariables ? devLevelName : GameInfo.LevelName;
        
        if((!useDevVariables && GameInfo.requestAnalysis) || (useDevVariables && loadFromAnalyse))
            LevelLoader.Instance.LoadLevelFromAnalysis(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
