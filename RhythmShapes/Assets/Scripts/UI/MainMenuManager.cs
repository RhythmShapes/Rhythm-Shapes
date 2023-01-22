using TMPro;
using UnityEngine;
using UnityEngine.UI;
using utils;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionMenuCanvas;
    [SerializeField] private GameObject musicSelectionMenuCanvas;
    [SerializeField] private GameObject calibrationMenuCanvas;
    [SerializeField] private TextMeshProUGUI offsetTextTMP;
    [SerializeField] private GameObject difficultyCanvas;

    public void StartGame()
    { 
        // Debug.Log("StartGame");
        mainMenuCanvas.SetActive(false);
        musicSelectionMenuCanvas.SetActive(true);
        SetOffsetText();
    }

    public void LaunchEditor()
    {
        GameInfo.IsNewLevel = true;
        SceneTransition.Instance.LoadScene(2);
    }
    
    public void ShowOptions()
    {
        // Debug.Log("ShowOptions");
        mainMenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(true);
    }
    
    public void HideOptions()
    {
        // Debug.Log("HideOptions");
        optionMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
    }
    
    public void StartCalibration()
    { 
        // Debug.Log("StartCalibration");
        SceneTransition.Instance.LoadScene(3);
    }
    
    public void CalibrationPlus1()
    { 
        // Debug.Log("CalibrationPlus1");
        GameInfo.InputCalibration += 0.001f;
        if (GameInfo.InputCalibration < -0.05f)
        {
            GameInfo.InputCalibration = -0.05f;
        }
        else if (GameInfo.InputCalibration > 0.2f)
        {
            GameInfo.InputCalibration = 0.2f;
        }
        SetOffsetText();
    }
    
    public void CalibrationMinus1()
    { 
        // Debug.Log("CalibrationPlus1");
        GameInfo.InputCalibration -= 0.001f;
        if (GameInfo.InputCalibration < -0.05f)
        {
            GameInfo.InputCalibration = -0.05f;
        }
        else if (GameInfo.InputCalibration > 0.2f)
        {
            GameInfo.InputCalibration = 0.2f;
        }
        SetOffsetText();
    }
    
    public void SetOffsetText()
    { 
        PlayerPrefsManager.SetPref("InputOffset",GameInfo.InputCalibration);
        var text = Mathf.RoundToInt(GameInfo.InputCalibration*1000).ToString() + " ms";
        offsetTextTMP.text = text;
    }
    
    public void ShowCalibration()
    {
        // Debug.Log("ShowCalibration");
        optionMenuCanvas.SetActive(false);
        calibrationMenuCanvas.SetActive(true);
        SetupCalibrationText();
    }
    
    public void HideCalibration()
    {
        // Debug.Log("HideCalibration");
        calibrationMenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(true);
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

    private void SetupCalibrationText()
    {
        if (GameInfo.InputCalibration < -0.05f)
        {
            GameInfo.InputCalibration = -0.05f;
        }
        else if (GameInfo.InputCalibration > 0.2f)
        {
            GameInfo.InputCalibration = 0.2f;
        }
        var text = Mathf.RoundToInt(GameInfo.InputCalibration*1000).ToString() + " ms";
        offsetTextTMP.text = text;
    }
    
    public void ShowPresetPanel()
    {
        // Debug.Log("ShowPresetPanel");
        musicSelectionMenuCanvas.SetActive(false);
        difficultyCanvas.SetActive(true);
    }
    
    public void HidePresetPanel()
    {
        // Debug.Log("HidePresetPanel");
        difficultyCanvas.SetActive(false);
        musicSelectionMenuCanvas.SetActive(true);
    }
}
