using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] [Min(0)] private float timeBefore;
    [SerializeField] [Min(0)] private float timeAfter;

    public float time
    {
        get => _time;
        set
        {
            if (audioSource != null && audioSource.clip != null)
            {
                _time = Mathf.Clamp(value, 0f - timeBefore, audioSource.clip.length + timeAfter);
                audioSource.time = Mathf.Clamp(value, 0f, audioSource.clip.length);

                if (_time < 0f) audioSource.Stop();
            }
        }
    }

    public float length => audioSource.clip.length + timeAfter + GameInfo.AudioCalibration;
    
    public float totalLength => timeBefore + length;

    public float audioTime => audioSource.time;

    public bool isPlaying { get; private set; }
    
    public bool isPaused { get; private set; }
    
    public float volume
    {
        get => audioSource.volume;
        set => audioSource.volume = value;
    }

    public bool mute
    {
        get => audioSource.mute;
        set => audioSource.mute = value;
    }

    public AudioClip clip => audioSource.clip;

    private float _time;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        CompleteReset();
    }

    private void Update()
    {
        if(!isPlaying || isPaused) return;

        float newTime = _time;
        newTime = _time >= 0f && _time <= audioSource.clip.length && audioSource.isPlaying ? audioSource.time : newTime + Time.deltaTime;

        if (newTime >= length)
        {
            Stop();
            CompleteReset();
            return;
        }

        _time = newTime;
    }

    public void Play()
    {
        if (_time >= 0f && _time < audioSource.clip.length)
            PlayOrUnpause();

        isPlaying = true;
        isPaused = false;
    }

    private void PlayOrUnpause()
    {
        if (isPaused) audioSource.UnPause();
        else audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
        CompleteReset();
    }

    public void Pause()
    {
        audioSource.Pause();
        isPlaying = false;
        isPaused = true;
    }

    public void Reset()
    {
        time = 0f - timeBefore;
    }

    private void CompleteReset()
    {
        isPlaying = false;
        isPaused = false;
        Reset();
    }
}