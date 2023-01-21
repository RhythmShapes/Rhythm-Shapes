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
        if (useDevVariables && (index == 3 || index == 4))
            levelLoader.LoadLevelFromRessourcesFolder(levelName);
        else
            levelLoader.LoadLevelFromFile(levelName);
    }
}
