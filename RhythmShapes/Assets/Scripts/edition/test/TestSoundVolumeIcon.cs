using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edition.test
{
    public class TestSoundVolumeIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Slider sliderVolume;
        [SerializeField] private Sprite volumeImage;
        [SerializeField] private Sprite muteImage;

        private Image _image;
        private float _lastVolume = 0f;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnVolumeChanged(float volume)
        {
            if (volume > 0f)
            {
                _image.sprite = volumeImage;
                _lastVolume = 0f;
                return;
            }
            
            _image.sprite = muteImage;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (sliderVolume.value > 0f && _lastVolume.Equals(0f))
            {
                _lastVolume = sliderVolume.value;
                sliderVolume.value = 0f;
                OnVolumeChanged(0f);
                return;
            }
            
            sliderVolume.value = _lastVolume;
            OnVolumeChanged(sliderVolume.value);
        }
    }
}
