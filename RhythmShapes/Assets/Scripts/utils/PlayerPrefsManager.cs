namespace utils
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerPrefsManager : MonoBehaviour
    {
        [SerializeField] private Slider effectsVolume;
        [SerializeField] private Slider musicVolume;

        private void Start()
        {
            effectsVolume.value = GetPref("EffectsVolume", 1f);
            musicVolume.value = GetPref("MusicVolume", 1f);
            GameInfo.InputCalibration = GetPref("InputOffset",0f);
            GameInfo.AudioCalibration = GetPref("AudioOffset", 0.05f);
            if(musicVolume != null)
                effectsVolume.onValueChanged.Invoke(effectsVolume.value);
            if(musicVolume != null)
                musicVolume.onValueChanged.Invoke(musicVolume.value);
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