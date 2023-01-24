using System;
using System.Collections.Generic;
using edition.messages;
using edition.timeLine;
using models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using utils;
using utils.XML;

namespace edition.test
{
    public class TestManager : MonoBehaviour
    {
        [SerializeField] private TestLine testLine;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioPlayer audioPlayer;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource hitSoundSource;
        [SerializeField] private RectTransform widthRef;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private UnityEvent<LevelDescription> onTestStart;
        [SerializeField] private UnityEvent onTestStop;
        
        public static bool IsTestRunning { get; private set; } = false;

        private bool _isPaused;
        private float _time;

        private void Start()
        {
            ResetCursor();
            audioPlayer.volume = 1f;
            ChangeVolume(PlayerPrefsManager.GetPref("MusicVolume", 1f));
            onTestStart ??= new UnityEvent<LevelDescription>();
            onTestStop ??= new UnityEvent();
        }

        public void StartTest()
        {
            if (_isPaused)
            {
                audioPlayer.Play();
                Time.timeScale = 1f;
                _isPaused = false;
                return;
            }
            
            if(audioPlayer.isPlaying)
                return;

            if (!EditorModel.HasLevelSet())
            {
                NotificationsManager.ShowError("A music has to be analysed before testing.");
                return;
            }

            audioPlayer.mute = false;
            audioPlayer.Stop();
            
            testLine.Init(ShapeTimeLine.GetPosX(audioPlayer.time));
            testLine.gameObject.SetActive(true);
            
            audioPlayer.Play();
            IsTestRunning = true;
            UpdateCursor(_time);
        }
        
        public void PauseTest()
        {
            if (audioPlayer.isPlaying)
            {
                audioPlayer.Pause();
                Time.timeScale = 0f;
                _isPaused = true;
            }
        }
        
        public void StopTest()
        {
            if (audioPlayer.isPlaying || _isPaused)
            {
                _time = audioPlayer.time;
                audioPlayer.Stop();
                _isPaused = false;
                IsTestRunning = false;
                Time.timeScale = 1f;
                GameModel.Instance.Reset();
                onTestStop.Invoke();
            }
        }
        
        public void ResetCursor()
        {
            UpdateCursor(0f);
        }

        public void ChangeVolume(float volume)
        {
            volumeSlider.value = volume;
            hitSoundSource.volume = volume;
            Utils.SetAudioMixerVolume(audioMixer, volume);
        }

        private void Update()
        {
            if (IsTestRunning)
            {
                if (!audioPlayer.isPlaying && !_isPaused)
                {
                    _time = audioPlayer.length;
                    _isPaused = false;
                    IsTestRunning = false;
                    Invoke("ResetModel", .1f);
                    onTestStop.Invoke();
                    return;
                }
                
                float posX = ShapeTimeLine.GetPosX(audioPlayer.time - GameInfo.AudioCalibration - GameModel.Instance.BadPressedWindow*2);
                testLine.UpdatePosX(posX);
                
                if(audioPlayer.isPlaying)
                    UpdateScroll(posX);
            }
        }

        private void ResetModel()
        {
            GameModel.Instance.Reset();
        }

        private void UpdateScroll(float posX)
        {
            var viewWidth = widthRef.rect.width;
            float widthOffset = TimeLine.RealWidth - viewWidth;
            float viewStart = scrollbar.value * widthOffset;
            float viewEnd = viewStart + viewWidth;

            if (posX < viewStart || posX > viewEnd)
                scrollbar.value = Mathf.Clamp(posX / widthOffset, 0f, 1f);
        }

        public void UpdateCursor()
        {
            float posX = ShapeTimeLine.GetPosX(audioPlayer.isPlaying ? audioPlayer.time : _time);
            testLine.UpdatePosX(posX);
        }

        public void UpdateCursor(float time)
        {
            if (IsTestRunning)
            {
                LevelDescription level = EditorModel.GetCurrentLevel();
                LevelDescription newLevel = new LevelDescription();
                List<ShapeDescription> shapes = new List<ShapeDescription>();

                foreach (var shape in level.shapes)
                {
                    if(shape.timeToPress > time)
                        shapes.Add(shape);
                }

                newLevel.title = level.title;
                newLevel.shapes = shapes.ToArray();
                GameModel.Instance.Reset();
                onTestStart.Invoke(newLevel);
            }

            testLine.UpdatePosX(ShapeTimeLine.GetPosX(time));
            _time = time;

            if (audioPlayer.isPlaying || _isPaused)
                audioPlayer.time = time + GameInfo.AudioCalibration + GameModel.Instance.BadPressedWindow * 2;
        }
    }
}
