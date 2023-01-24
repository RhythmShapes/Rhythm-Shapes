using models;
using UnityEngine;
using UnityEngine.Events;

public class CheckEndGame : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameEnded;
    private AudioSource _audioSource;
    private AudioPlayer _audioPlayer;
    private float _audioLength;
    private float _timeCounter = 0;

    private void Awake()
    {
        onGameEnded ??= new UnityEvent();
    }

    public void Init()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
        _audioLength = _audioPlayer.length;
        _timeCounter = 0;
    }

    private void Update()
    {
        if (!GameModel.Instance.isGamePaused)
        {
            _timeCounter = _audioPlayer.time > 0 ? _audioPlayer.time : _timeCounter;
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
