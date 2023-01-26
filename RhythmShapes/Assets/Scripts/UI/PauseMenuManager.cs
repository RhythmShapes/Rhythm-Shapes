using models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using utils;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private AudioMixer audioMixer;

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
        SceneTransition.Instance.BackToMainMenu();
    }

    public void ChangeMusicVolume(float volume)
    {
        Utils.SetAudioMixerVolume(audioMixer, volume);
    }
}
