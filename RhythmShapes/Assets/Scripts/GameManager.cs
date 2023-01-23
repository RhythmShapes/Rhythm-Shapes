using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables;
    [SerializeField] private string devLevelName;

    private void Start()
    {
        string levelName = useDevVariables ? devLevelName : GameInfo.LevelName;
        int index = SceneManager.GetActiveScene().buildIndex;

        if (GameInfo.RequestAnalysis && GameInfo.IsNewLevel && SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("LoadLevelFromAnalysis");
            GameInfo.RequestAnalysis = false;
            levelLoader.LoadLevelFromAnalysis(PresetDifficulty.Instance.musicPath);
        }
        else if (useDevVariables && index is 3 or 4 or 5)
            levelLoader.LoadLevelFromRessourcesFolder(levelName);
        else
            levelLoader.LoadLevelFromFile(levelName);
    }
}
