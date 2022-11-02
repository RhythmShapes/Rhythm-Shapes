using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private UnityEvent onGameResumed;
    
    private void Awake()
    {
        onGameResumed ??= new UnityEvent();
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
        audioSource.Pause();
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        audioSource.Play();
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        onGameResumed.Invoke();
    }
    
    public void RestartGame()
    {
        // Debug.Log("RestartGame");
        // Time.timeScale = 1;
        // onGameResumed.Invoke();
        // pauseMenuCanvas.SetActive(false);
        // SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        Time.timeScale = 1;
        onGameResumed.Invoke();
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
