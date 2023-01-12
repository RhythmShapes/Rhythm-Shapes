using System;
using TMPro;
using UnityEngine;
using utils;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionMenuCanvas;
    [SerializeField] private GameObject calibrationMenuCanvas;
    [SerializeField] private GameObject musicSelectionMenuCanvas;
    [SerializeField] private TextMeshProUGUI offsetTextTMP;
    public void StartGame()
    { 
        // Debug.Log("StartGame");
        mainMenuCanvas.SetActive(false);
        musicSelectionMenuCanvas.SetActive(true);
        SetOffsetText();
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
        // Debug.Log("StartGame");
        // SceneTransition.Instance.LoadScene(2);
    }
    
    public void CalibrationPlus1()
    { 
        // Debug.Log("CalibrationPlus1");
        GameInfo.Calibration += 0.001f;
        SetOffsetText();
    }
    
    public void CalibrationMinus1()
    { 
        // Debug.Log("CalibrationPlus1");
        GameInfo.Calibration -= 0.001f;
        
        SetOffsetText();
    }
    
    public void SetOffsetText()
    { 
        PlayerPrefsManager.Instance.SetPref("InputOffset",GameInfo.Calibration);
        var text = Mathf.RoundToInt(GameInfo.Calibration*1000).ToString() + " ms";
        offsetTextTMP.text = text;
    }
    
    public void ShowCalibration()
    {
        // Debug.Log("ShowCalibration");
        optionMenuCanvas.SetActive(false);
        calibrationMenuCanvas.SetActive(true);
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

    private void Awake()
    {
        var text = Mathf.RoundToInt(GameInfo.Calibration*1000).ToString() + " ms";
        offsetTextTMP.text = text;
    }
}
