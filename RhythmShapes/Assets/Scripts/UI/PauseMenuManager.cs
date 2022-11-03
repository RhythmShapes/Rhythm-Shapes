using System.Collections;
using System.Collections.Generic;
using models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;

    private void Awake()
    {
        LoadPlayerPrefs();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame");
        mainCamera.position = Vector3.zero;
        pauseMenuCanvas.SetActive(true);
        GameModel.Instance.PauseGame();
    }
    
    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        mainCamera.position = -10*Vector3.forward;
        pauseMenuCanvas.SetActive(false);
        GameModel.Instance.UnPauseGame();
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame");
        mainCamera.position = -10*Vector3.forward;
        pauseMenuCanvas.SetActive(false);
        GameModel.Instance.RestartGame();
    }

    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        mainCamera.position = -10*Vector3.forward;
        pauseMenuCanvas.SetActive(false);
        SceneManager.LoadScene("MenuScene");
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
