using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionMenuCanvas;
    [SerializeField] private GameObject musicSelectionMenuCanvas;
    public void StartGame()
    { 
        // Debug.Log("StartGame");
        mainMenuCanvas.SetActive(false);
        musicSelectionMenuCanvas.SetActive(true);
    }
    
    public void ShowOptions()
    {
        // Debug.Log("ShowOptions");
        mainMenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(true);
    }
    
    public void HideOptions()
    {
        // Debug.Log("ShowOptions");
        optionMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
    }
    
    public void QuitGame()
    {
        // Debug.Log("QuitGame");
        SceneTransition.Instance.Quit();
    }
    
    public void BackToMainMenu()
    { 
        // Debug.Log("BackToMainMenu");
        musicSelectionMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}
