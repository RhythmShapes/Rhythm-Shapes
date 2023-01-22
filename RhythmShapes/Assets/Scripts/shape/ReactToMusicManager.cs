﻿using AudioAnalysis;
using UnityEngine;

namespace shape
{
    public class ReactToMusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private float maxScale;
        
        private static float _maxScale = 1f;
        private static float _signalLevel = 1f;
        private static AudioSource _source;

        private const int SampleLength = 2048;
        
        private float[] _signal = new float[SampleLength];
        private float _oldSignalLevel = 1f;
        private float _maxSignalLevel = 1f;

        private void Start()
        {
            _maxScale = maxScale;
            _source = source;
        }

        public static float GetScale(float minScale)
        {
            return !_source.isPlaying ? minScale : Mathf.Clamp(_signalLevel, minScale, _maxScale);
        }
        
        private void Update()
        {
            source.GetSpectrumData(_signal,0, FFTWindow.Rectangular);
            _signal = AudioTools.Normalize(_signal);
            _signalLevel = 0f;
            
            foreach (var level in _signal)
            {
                if(float.IsNaN(level)) continue;
                
                _signalLevel += level;
            }

            if(_signalLevel < _oldSignalLevel)
                _signalLevel = 0.9f*_oldSignalLevel;

            if (_signalLevel > _maxSignalLevel)
                _maxSignalLevel = _signalLevel;

            _signalLevel = _signalLevel / _maxSignalLevel + 0.8f;
            _oldSignalLevel = _signalLevel;
        }
    }
}