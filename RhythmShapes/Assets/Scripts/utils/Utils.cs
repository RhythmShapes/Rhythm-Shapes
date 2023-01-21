using System;
using UnityEngine;
using UnityEngine.Audio;

namespace utils
{
    public static class Utils
    {
        private const int TimeRound = 3;
        private const string AudioMixerVolumeParameter = "Volume";

        public static float RoundTime(float time)
        {
            return (float) Math.Round(time, TimeRound);
        }

        public static void SetAudioMixerVolume(AudioMixer audioMixer, float volume)
        {
            volume = Mathf.Clamp(volume, 0.00001f, 1f);
            audioMixer.SetFloat(AudioMixerVolumeParameter, Mathf.Log10(volume) * 20);
        }
    }
}