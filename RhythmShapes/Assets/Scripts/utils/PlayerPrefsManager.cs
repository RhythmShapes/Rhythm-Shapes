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
            effectsVolume.onValueChanged.Invoke(effectsVolume.value);
            musicVolume.onValueChanged.Invoke(musicVolume.value);
        }

        private void SetPref(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        private float GetPref(string key, float placeholder)
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