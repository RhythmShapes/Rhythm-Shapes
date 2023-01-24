using models;
using UnityEngine;
using UnityEngine.Events;

public class CheckEndGame : MonoBehaviour
{

    [SerializeField] private UnityEvent onGameEnded;
    private AudioSource _audioSource;
    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        onGameEnded ??= new UnityEvent();
    }

    public void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    private void Update()
    {
        if (!GameModel.Instance.isGamePaused)
        {
            if (_audioPlayer.time > _audioPlayer.length + 2 * GameModel.Instance.BadPressedWindow)
                onGameEnded.Invoke();
        }
        
    }

    public void ResetTimeCounter()
    {
        //ignored
    }
}
