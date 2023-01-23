using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
//[Obsolete]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] [Min(0)] private float timeBefore;
    [SerializeField] [Min(0)] private float timeAfter;

    public float time
    {
        get => _time;
        set
        {
            if (_audioSource != null && _audioSource.clip != null)
            {
                _time = Mathf.Clamp(value, 0f - timeBefore, _audioSource.clip.length + timeAfter);
                _audioSource.time = Mathf.Clamp(value, 0f, _audioSource.clip.length);

                if (_time < 0f) _audioSource.Stop();
            }
        }
    }

    public float length => timeBefore + _audioSource.clip.length + timeAfter;

    public float audioTime => _audioSource.time;

    public bool isPlaying { get; private set; }
    
    public bool isPaused { get; private set; }
    
    public float volume
    {
        get => _audioSource.volume;
        set => _audioSource.volume = value;
    }

    public AudioClip clip => _audioSource.clip;

    private AudioSource _audioSource;
    private float _time;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        CompleteReset();
    }

    private void Update()
    {
        if(!isPlaying) return;

        float newTime = _time;
        newTime = _time >= 0f && _time <= _audioSource.clip.length ? _audioSource.time : newTime + Time.deltaTime;

        if (_time >= 0f && !_audioSource.isPlaying && !isPaused)
            PlayOrUnpause();

        if (newTime >= _audioSource.clip.length + timeAfter) CompleteReset();
        else _time = newTime;
    }

    public void Play()
    {
        if (_time >= 0f)
            PlayOrUnpause();

        isPlaying = true;
        isPaused = false;
    }

    private void PlayOrUnpause()
    {
        if (isPaused) _audioSource.UnPause();
        else _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
        CompleteReset();
    }

    public void Pause()
    {
        _audioSource.Pause();
        isPlaying = false;
        isPaused = true;
    }

    public void Reset()
    {
        time = 0f - timeBefore;
    }

    private void CompleteReset()
    {
        Reset();
        isPlaying = false;
        isPaused = false;
    }
}