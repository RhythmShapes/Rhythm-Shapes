using AudioAnalysis;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace shape
{
    public class ReactToMusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private float minScale = 1f;
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private float peakScale = .1f;
        [SerializeField] private float factor = 1f;
        [SerializeField] [Range(1, SampleLength)] private int keptFrequency;
        [SerializeField] private float reduceRate = 1f;

        private const int SampleLength = 2048;
        
        private float[] _signal = new float[SampleLength];
        private float _oldSignalLevel = 1f;
        private float _maxSignalLevel = 1f;

        public static float SignalLevel { get; private set; } = 1f;

        public static float GetScale(float minScale, float maxScale)
        {
            return Mathf.Clamp(SignalLevel * (maxScale - minScale) + minScale, minScale, maxScale);
        }
        
        private void Update()
        {
            source.GetSpectrumData(_signal,0, FFTWindow.Rectangular);
            _signal = AudioTools.Normalize(_signal);
            SignalLevel = 0f;
            
            for (int i = 0; i < keptFrequency; i++)
            {
                if(float.IsNaN(_signal[i])) continue;
                SignalLevel += _signal[i];
            }

            /*if(_signalLevel < _oldSignalLevel)
                _signalLevel = 0.9f * _oldSignalLevel;*/

            if (SignalLevel > _maxSignalLevel)
                _maxSignalLevel = SignalLevel;

            float newSignal = (SignalLevel / _maxSignalLevel + minScale) * factor;
            SignalLevel = newSignal >= peakScale ? newSignal : minScale;

            //Debug.Log(_signalLevel+"  "+_oldSignalLevel);
            if (SignalLevel <= _oldSignalLevel && SignalLevel > minScale)
                /*SignalLevel = 0.9f * _oldSignalLevel;*/SignalLevel -= reduceRate * Time.deltaTime;

            SignalLevel = Mathf.Min(SignalLevel, maxScale);
            _oldSignalLevel = SignalLevel;
        }
    }
}