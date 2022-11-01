using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables;
    [SerializeField] private bool loadFromAnalyse;
    [SerializeField] private string devLevelName;
    
    public string LevelName { get; set; }
    public bool Analyse { get; set; }

    private void Start()
    {
        string levelName = useDevVariables ? devLevelName : LevelName;
        
        if((!useDevVariables && Analyse) || (useDevVariables && loadFromAnalyse))
            LevelLoader.Instance.LoadLevelFromAnalyse(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
