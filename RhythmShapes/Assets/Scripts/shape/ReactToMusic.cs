using AudioAnalysis;
using UnityEngine;
using UnityEngine.Audio;

namespace shape
{
    public class ReactToMusic : MonoBehaviour
    {
        [SerializeField] private float maxScale;
        
        private AudioSource source;
        private float[] signal = new float[2048];
        private float signalLevel;
        private float oldSignalLevel;
        private float maxSignalLevel = 1;
        private float _originalScale = 1f;

        private void Start()
        {
            source = GameObject.Find("GameplayManager").GetComponent<AudioSource>();
            _originalScale = transform.localScale.x;
        }
        
        void Update()
        {
            source.GetSpectrumData(signal,0, FFTWindow.Rectangular);
            signal = AudioTools.Normalize(signal);
            signalLevel = 0;
            for(int i = 0; i < signal.Length; i++)
            {
                signalLevel += signal[i];
            }

            if(signalLevel < oldSignalLevel)
            {
                signalLevel = 0.9f*oldSignalLevel;
            }

            if (signalLevel > maxSignalLevel)
            {
                maxSignalLevel = signalLevel;
            }
            signalLevel = signalLevel / maxSignalLevel + 0.8f;
            if (!float.IsNaN(signalLevel))
            {
                signalLevel = Mathf.Clamp(signalLevel, _originalScale, maxScale);
                transform.localScale = new Vector3(signalLevel, signalLevel, signalLevel);
            }
            oldSignalLevel = signalLevel;
        }
    }
}
