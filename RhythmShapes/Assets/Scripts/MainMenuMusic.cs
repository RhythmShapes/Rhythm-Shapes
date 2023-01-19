using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = PlayerPrefsManager.GetPref("MusicVolume", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
