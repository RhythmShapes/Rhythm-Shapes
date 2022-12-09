using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private int mainMenuBuildIndex;

    public static SceneTransition Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
        else
            Instance = this;
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void LoadScene(string sceneName)
    {
        LoadScene(SceneManager.GetSceneByName(sceneName).buildIndex);
    }
    
    public void LoadNextScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.DeleteKey("spawnPosition_x");
        PlayerPrefs.DeleteKey("spawnPosition_y");
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void BackToMainMenu()
    {
        LoadScene(mainMenuBuildIndex);
        PlayerPrefs.DeleteKey("spawnPosition_x");
        PlayerPrefs.DeleteKey("spawnPosition_y");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
