using UI;
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
        FileExplorer explorer = FindObjectOfType<FileExplorer>();
        string levelName = useDevVariables ? devLevelName : explorer.levelName;
        
        if((!useDevVariables && explorer.requestAnalysis) || (useDevVariables && loadFromAnalyse))
            LevelLoader.Instance.LoadLevelFromAnalysis(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
