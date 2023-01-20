using System.Collections.Generic;
using edition.messages;
using edition.timeLine;
using models;
using UnityEngine;
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
            ChangeVolume(PlayerPrefsManager.GetPref("volume", 1f));
            onTestStart ??= new UnityEvent<LevelDescription>();
            onTestStop ??= new UnityEvent();
        }

        public void StartTest()
        {
            if (_isPaused)
            {
                audioSource.UnPause();
                Time.timeScale = 1f;
                _isPaused = false;
                return;
            }
            
            if(audioSource.isPlaying)
                return;

            if (!EditorModel.HasLevelSet())
            {
                NotificationsManager.ShowError("A music has to be analysed before testing.");
                return;
            }

            audioSource.mute = false;
            audioSource.Stop();
            
            testLine.Init(ShapeTimeLine.GetPosX(audioSource.time));
            testLine.gameObject.SetActive(true);
            
            audioSource.Play();
            IsTestRunning = true;
            UpdateCursor(_time);
        }
        
        public void PauseTest()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                Time.timeScale = 0f;
                _isPaused = true;
            }
        }
        
        public void StopTest()
        {
            if (audioSource.isPlaying || _isPaused)
            {
                _time = audioSource.time;
                audioSource.Stop();
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
            audioSource.volume = volume;
        }

        private void Update()
        {
            if (IsTestRunning)
            {
                if (!audioSource.isPlaying && !_isPaused)
                {
                    _time = audioSource.clip.length;
                    IsTestRunning = false;
                }
                
                float posX = ShapeTimeLine.GetPosX(audioSource.time);
                testLine.UpdatePosX(posX);
                
                if(audioSource.isPlaying)
                    UpdateScroll(posX);
            }
        }

        private void UpdateScroll(float posX)
        {
            var viewWidth = widthRef.rect.width;
            float widthOffset = TimeLine.RealWidth - viewWidth;
            float viewStart = scrollbar.value * widthOffset;
            float viewEnd = viewStart + viewWidth;

            if (posX < viewStart || posX > viewEnd)
                scrollbar.value = Mathf.Clamp((posX) / widthOffset, 0f, 1f);
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

            if (audioSource.isPlaying || _isPaused)
                audioSource.time = time;
        }
    }
}
