using UnityEngine;
using UnityEngine.SceneManagement;

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
        else if (useDevVariables && SceneManager.GetActiveScene().buildIndex==2)
            LevelLoader.Instance.LoadLevelFromRessourcesFolder(levelName);
        else
            LevelLoader.Instance.LoadLevelFromFile(levelName);
    }
}
