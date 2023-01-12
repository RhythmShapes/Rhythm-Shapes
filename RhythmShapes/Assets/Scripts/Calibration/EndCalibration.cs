using System.Collections;
using System.Collections.Generic;
using models;
using TMPro;
using UnityEngine;
using utils;

public class EndCalibration : MonoBehaviour
{
    [SerializeField] private GameObject endCalibrationCanvas; 
    [SerializeField] private TextMeshProUGUI offsetTextTMP;
    
    
    public void ShowEndLevelMenu()
    {
        endCalibrationCanvas.SetActive(true);
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame");
        endCalibrationCanvas.SetActive(false);
        GameModel.Instance.RestartGame();
    }

    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        endCalibrationCanvas.SetActive(false);
        SceneTransition.Instance.BackToMainMenu();
    }
    
    public void SetOffsetText()
    {
        var text = Mathf.RoundToInt(GameInfo.Calibration*1000).ToString() + " ms";
        offsetTextTMP.text = text;
    }
}
