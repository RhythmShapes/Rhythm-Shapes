using UnityEngine.Audio;

namespace utils
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerPrefsManager : MonoBehaviour
    {
        [SerializeField] private Slider effectsVolume;
        [SerializeField] private Slider musicVolume;

        public const string ScoreSuffix = ":score";
        public const string ComboSuffix = ":combo";

        private void Start()
        {
            GameInfo.InputCalibration = GetPref("InputOffset",0.05f);
            GameInfo.AudioCalibration = GetPref("AudioOffset", 0);

            if (effectsVolume != null)
            {
                effectsVolume.value = GetPref("EffectsVolume", 1f);
                effectsVolume.onValueChanged.Invoke(effectsVolume.value);
            }

            if (musicVolume != null)
            {
                musicVolume.value = GetPref("MusicVolume", 1f);
                musicVolume.onValueChanged.Invoke(musicVolume.value);
            }
        }

        public static bool HasPref(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void SetPref(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        public static float GetPref(string key, float placeholder)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : placeholder;
        }

        public static void DeletePref(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public void UpdateEffectVolume()
        {
            SetPref("EffectsVolume", effectsVolume.value);
        }

        public void UpdateMusicVolume()
        {
            SetPref("MusicVolume", musicVolume.value);
        }
    }
}