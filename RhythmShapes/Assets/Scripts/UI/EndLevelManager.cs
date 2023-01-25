using System;
using System.Collections;
using System.Collections.Generic;
using models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject endLevelMenuCanvas; 
    [SerializeField] private TextMeshProUGUI levelTitleTMP;
    [SerializeField] private TextMeshProUGUI bestComboTMP;
    [SerializeField] private TextMeshProUGUI rankTitleTMP;
    [SerializeField] private TextMeshProUGUI scoreTitleTMP;
    [SerializeField] private TextMeshProUGUI perfectCounterTMP;
    [SerializeField] private TextMeshProUGUI goodCounterTMP;
    [SerializeField] private TextMeshProUGUI okCounterTMP;
    [SerializeField] private TextMeshProUGUI badCounterTMP;
    [SerializeField] private TextMeshProUGUI missCounterTMP;
    [SerializeField] private TextMeshProUGUI accuracyTMP;
    [SerializeField] private RawImage musicImage;
    [SerializeField] private Transform mainCamera;

    [Header("Dev variables")]
    [Space]
    [SerializeField] private bool useDevVariables = false;
    [SerializeField] private string devLevelName = "LEVELTEST";

    public void ShowEndLevelMenu()
    {
        mainCamera.position = Vector3.zero;
        endLevelMenuCanvas.SetActive(true);
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame");
        mainCamera.position = -10*Vector3.forward;
        endLevelMenuCanvas.SetActive(false);
        GameModel.Instance.RestartGame();
    }

    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        mainCamera.position = -10*Vector3.forward;
        endLevelMenuCanvas.SetActive(false);
        SceneTransition.Instance.BackToMainMenu();
    }
    
    
    public void SetLevelTitleText()
    {
        var text = useDevVariables ? devLevelName : GameInfo.LevelName;
        levelTitleTMP.text = text;
    }
    
    public void SetRankTitleText(string text)
    {
        rankTitleTMP.text = "Rank : "+text;
    }
    
    public void SetScoreTitleText()
    {
        var text = ScoreManager.Instance.Score.ToString();
        scoreTitleTMP.text = "Score : "+text;
    }
    
    public void SetPerfectCounterText()
    {
        var text = ScoreManager.Instance.PerfectCounter.ToString();
        perfectCounterTMP.text = text + " Perfect";
    }
    
    public void SetGoodCounterText()
    {
        var text = ScoreManager.Instance.GoodCounter.ToString();
        goodCounterTMP.text = text + " Good";
    }
    
    public void SetOkCounterText()
    {
        var text = ScoreManager.Instance.OkCounter.ToString();
        okCounterTMP.text = text + " Ok";
    }
    
    public void SetBadCounterText()
    {
        var text = ScoreManager.Instance.BadCounter.ToString();
        badCounterTMP.text = text + " Bad";
    }
    
    public void SetMissCounterText()
    {
        var text = ScoreManager.Instance.MissCounter.ToString();
        missCounterTMP.text = text + " Miss";
    }
    
    public void SetMusicImage(Texture2D texture)
    {
        musicImage.texture = texture;
    }

    public void SetBestCombo()
    {
        var text = ScoreManager.Instance.BestCombo.ToString();
        bestComboTMP.text = "Best Combo : "+text;
    }
    
    public void SetAccuracy()
    {
        var text = ScoreManager.Instance.GetAccuracy()*100;
        accuracyTMP.text = text.ToString("F1") + "%";
    }
    
}
