using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables;
    [SerializeField] private bool loadFromAnalyse;
    [SerializeField] private string devLevelName;
    public bool Analyse { get; set; } = true;

    private void Start()
    {
        string levelName = useDevVariables ? devLevelName : FindObjectOfType<FileExplorer>().levelName;
        
        if((!useDevVariables && Analyse) || (useDevVariables && loadFromAnalyse))
            LevelLoader.Instance.LoadLevelFromAnalysis(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
