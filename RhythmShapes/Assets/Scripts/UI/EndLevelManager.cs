using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject endLevelMenuCanvas; 
    [SerializeField] private TextMeshProUGUI levelTitleTMP;
    [SerializeField] private TextMeshProUGUI rankTitleTMP;
    [SerializeField] private TextMeshProUGUI scoreTitleTMP;
    [SerializeField] private TextMeshProUGUI perfectCounterTMP;
    [SerializeField] private TextMeshProUGUI goodCounterTMP;
    [SerializeField] private TextMeshProUGUI okCounterTMP;
    [SerializeField] private TextMeshProUGUI badCounterTMP;
    [SerializeField] private TextMeshProUGUI missCounterTMP;
    [SerializeField] private RawImage musicImage;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame");
        mainCamera.position = -10*Vector3.forward;
        endLevelMenuCanvas.SetActive(false);
        audioSource.Pause();
        audioSource.time = 0;
        LevelLoader.Instance.LoadLevelFromCurrentLevelDescription();
        // SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        mainCamera.position = -10*Vector3.forward;
        endLevelMenuCanvas.SetActive(false);
        SceneManager.LoadScene("MenuScene");
    }
    
    
    public void SetLevelTitleText(string text)
    {
        levelTitleTMP.text = text;
    }
    
    public void SetRankTitleText(string text)
    {
        rankTitleTMP.text = "RANK : "+text;
    }
    
    public void SetScoreTitleText(string text)
    {
        scoreTitleTMP.text = "SCORE : "+text;
    }
    
    public void SetPerfectCounterText(string text)
    {
        perfectCounterTMP.text = text + " PERFECT";
    }
    
    public void SetGoodCounterText(string text)
    {
        goodCounterTMP.text = text + " GOOD";
    }
    
    public void SetOkCounterText(string text)
    {
        okCounterTMP.text = text + " GOOD";
    }
    
    public void SetBadCounterText(string text)
    {
        badCounterTMP.text = text + " GOOD";
    }
    
    public void SetMissCounterText(string text)
    {
        missCounterTMP.text = text + " GOOD";
    }
    
    public void SetMusicImage(Texture2D texture)
    {
        musicImage.texture = texture;
    }
}
