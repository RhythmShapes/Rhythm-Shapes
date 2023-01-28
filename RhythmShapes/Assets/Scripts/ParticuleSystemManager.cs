using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using shape;
using ui;
using UnityEngine;

public class ParticuleSystemManager : MonoBehaviour
{
    public static ParticuleSystemManager Instance { get; private set; }

    [Header("RightTarget")] 
    [Header("SquareEffects")] 
    [Space] 
    [SerializeField] private ParticleSystem perfectSquareRight;
    [SerializeField] private ParticleSystem goodSquareRight;
    [SerializeField] private ParticleSystem okSquareRight;
    [SerializeField] private ParticleSystem badSquareRight;
    
    [Header("TopTarget")] 
    [Header("SquareEffects")] 
    [Space] 
    [SerializeField] private ParticleSystem perfectSquareTop;
    [SerializeField] private ParticleSystem goodSquareTop;
    [SerializeField] private ParticleSystem okSquareTop;
    [SerializeField] private ParticleSystem badSquareTop;
    
    [Header("LeftTarget")] 
    [Header("SquareEffects")] 
    [Space] 
    [SerializeField] private ParticleSystem perfectSquareLeft;
    [SerializeField] private ParticleSystem goodSquareLeft;
    [SerializeField] private ParticleSystem okSquareLeft;
    [SerializeField] private ParticleSystem badSquareLeft;

    [Header("BottomTarget")] 
    [Header("SquareEffects")] 
    [Space] 
    [SerializeField] private ParticleSystem perfectSquareBottom;
    [SerializeField] private ParticleSystem goodSquareBottom;
    [SerializeField] private ParticleSystem okSquareBottom;
    [SerializeField] private ParticleSystem badSquareBottom;
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
        else
            Instance = this;
    }

    public void PlayEffect(Target target, PressedAccuracy pressedAccuracy)
    {
        switch (pressedAccuracy)
        {
            case PressedAccuracy.Perfect:
                switch (target)
                {
                    case Target.Top: 
                        perfectSquareTop.Play();
                        break;
                    case Target.Right:
                        perfectSquareRight.Play();
                        break;
                    case Target.Left:
                        perfectSquareLeft.Play();
                        break;
                    case Target.Bottom:
                        perfectSquareBottom.Play();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(target), target, null);
                }
                break;
            case PressedAccuracy.Good:
                switch (target)
                {
                    case Target.Top: 
                        goodSquareTop.Play();
                        break;
                    case Target.Right:
                        goodSquareRight.Play();
                        break;
                    case Target.Left:
                        goodSquareLeft.Play();
                        break;
                    case Target.Bottom:
                        goodSquareBottom.Play();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(target), target, null);
                }
                break;
            case PressedAccuracy.Ok:
                switch (target)
                {
                    case Target.Top: 
                        okSquareTop.Play();
                        break;
                    case Target.Right:
                        okSquareRight.Play();
                        break;
                    case Target.Left:
                        okSquareLeft.Play();
                        break;
                    case Target.Bottom:
                        okSquareBottom.Play();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(target), target, null);
                }
                break;
            case PressedAccuracy.Bad:
                switch (target)
                {
                    case Target.Top: 
                        badSquareTop.Play();
                        break;
                    case Target.Right:
                        badSquareRight.Play();
                        break;
                    case Target.Left:
                        badSquareLeft.Play();
                        break;
                    case Target.Bottom:
                        badSquareBottom.Play();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(target), target, null);
                }
                break;
            case PressedAccuracy.Missed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pressedAccuracy), pressedAccuracy, null);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
