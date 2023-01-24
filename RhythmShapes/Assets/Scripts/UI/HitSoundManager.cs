using System;
using shape;
using ui;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class HitSoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip perfectClip;
        [SerializeField] private AudioClip goodClip;
        [SerializeField] private AudioClip okClip;
        [SerializeField] private AudioClip badClip;
        [SerializeField] private AudioClip missClip;

        private AudioSource _hitSoundSource;

        public static HitSoundManager Instance { get; private set; }

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        void Start()
        {
            _hitSoundSource = GetComponent<AudioSource>();
        }

        public void OnInputValidated(Target target, PressedAccuracy accuracy)
        {
            switch (accuracy)
            {
                case PressedAccuracy.Perfect:
                    _hitSoundSource.clip = perfectClip;
                    break;
                case PressedAccuracy.Good:
                    _hitSoundSource.clip = goodClip;
                    break;
                case PressedAccuracy.Ok:
                    _hitSoundSource.clip = okClip;
                    break;
                case PressedAccuracy.Bad:
                    _hitSoundSource.clip = badClip;
                    break;
                case PressedAccuracy.Missed:
                    _hitSoundSource.clip = missClip;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _hitSoundSource.Play();
        }
    }
}
