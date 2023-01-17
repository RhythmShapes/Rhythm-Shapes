using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using utils;
using utils.XML;

namespace edition
{
    public class TestManager : MonoBehaviour
    {
        [SerializeField] private TestLine testLine;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private RectTransform widthRef;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private UnityEvent<LevelDescription> onTestStart;

        private bool _isPaused;
        private bool _isTestRunning;
        private float _time;

        private void Start()
        {
            ResetCursor();
            ChangeVolume(PlayerPrefsManager.GetPref("volume", 1f));
            onTestStart ??= new UnityEvent<LevelDescription>();
        }

        public void StartTest()
        {
            if (_isPaused)
            {
                audioSource.UnPause();
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
            audioSource.time = _time;
            _isTestRunning = true;
            
            onTestStart.Invoke(EditorModel.GetCurrentLevel());
        }
        
        public void PauseTest()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
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
                _isTestRunning = false;
                //testLine.gameObject.SetActive(false);
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
            if (_isTestRunning)
            {
                if (!audioSource.isPlaying && !_isPaused)
                {
                    _time = audioSource.clip.length;
                    _isTestRunning = false;
                }
                
                float posX = ShapeTimeLine.GetPosX(audioSource.time);
                testLine.UpdatePosX(posX);
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
            testLine.UpdatePosX(ShapeTimeLine.GetPosX(time));
            _time = time;

            if (audioSource.isPlaying || _isPaused)
                audioSource.time = time;
        }
    }
}