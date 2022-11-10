using System;
using System.Collections;
using System.Collections.Generic;
using models;
using UnityEngine;
using UnityEngine.Events;

public class CheckEndGame : MonoBehaviour
{

    [SerializeField] private UnityEvent onGameEnded;
    private AudioSource _audioSource;
    private float _audioLength;
    private float _timeCounter = 0;

    private void Awake()
    {
        onGameEnded ??= new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioLength = _audioSource.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameModel.Instance.isGamePaused)
        {
            _timeCounter += Time.deltaTime;
            //Debug.Log("_timeCounter : "+_timeCounter);
            if (_timeCounter >= _audioLength + 2*GameModel.Instance.BadPressedWindow)
            {
                onGameEnded.Invoke();
            }
        }
        
    }

    public void ResetTimeCounter()
    {
        _timeCounter = 0;
    }
}
