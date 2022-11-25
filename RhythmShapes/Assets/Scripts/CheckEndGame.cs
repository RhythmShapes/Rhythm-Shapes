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

    public void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioLength = _audioSource.clip.length;
        _timeCounter = 0;
    }

    private void Update()
    {
        if (!GameModel.Instance.isGamePaused)
        {
            _timeCounter = _audioSource.time > 0 ? _audioSource.time : _timeCounter;
            _timeCounter += Time.deltaTime;

            if (_timeCounter - (_audioLength + 2*GameModel.Instance.BadPressedWindow) > 0)
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
