using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    [SerializeField] private AudioPlayer audioPlayer;
    [SerializeField] private Slider slider;

    private void Start() {
        slider.value = 0f;
    }

    private void Update() {
        slider.value = audioPlayer.time / audioPlayer.length;
    }
}
