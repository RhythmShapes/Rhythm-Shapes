using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionMenuCanvas;
    [SerializeField] private GameObject musicSelectionMenuCanvas;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;

    private void Awake()
    {
        LoadPlayerPrefs();
    }

    public void StartGame()
    { 
        Debug.Log("StartGame");
        // SceneManager.LoadScene("");
        mainMenuCanvas.SetActive(false);
        musicSelectionMenuCanvas.SetActive(true);
    }
    
    public void ShowOptions()
    {
        Debug.Log("ShowOptions");
        mainMenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(true);
    }
    
    public void HideOptions()
    {
        Debug.Log("ShowOptions");
        optionMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
    }
    
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }
    
    public void BackToMainMenu()
    { 
        Debug.Log("BackToMainMenu");
        
        musicSelectionMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    
    public void PlayLeveLTest()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SavePlayerMusicVolumePrefs()
    {
        float musicSliderVolume = musicSlider.value;
        PlayerPrefs.SetFloat("musicSliderVolume", musicSliderVolume);
        PlayerPrefs.Save();
        
        Debug.Log("PlayerMusicVolumePrefs Saved !!!");
    }
    
    public void SavePlayerEffectVolumePrefs()
    {
        float effectSliderVolume = effectSlider.value;
        PlayerPrefs.SetFloat("effectSliderVolume", effectSliderVolume);
        PlayerPrefs.Save();
        
        Debug.Log("PlayerEffectVolumePrefs Saved !!!");
    }
    
    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("musicSliderVolume") || PlayerPrefs.HasKey("effectSliderVolume"))
        {
            float musicSliderVolume = PlayerPrefs.GetFloat("musicSliderVolume", 1);
            float effectSliderVolume =  PlayerPrefs.GetFloat("effectSliderVolume", 1);
            musicSlider.value = musicSliderVolume;
            effectSlider.value = effectSliderVolume;
        }
        else
        {
            Debug.Log("No save available");
        }
    }
}
