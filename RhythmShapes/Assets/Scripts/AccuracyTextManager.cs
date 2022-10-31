using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using shape;
using TMPro;
using UnityEngine;

public class AccuracyTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topAccuracyText;
    [SerializeField] private TextMeshProUGUI rightAccuracyText;
    [SerializeField] private TextMeshProUGUI leftAccuracyText;
    [SerializeField] private TextMeshProUGUI bottomAccuracyText;
    [SerializeField] private string perfectMessage;
    [SerializeField] private string goodMessage;
    [SerializeField] private string okMessage;
    [SerializeField] private string badMessage;
    [SerializeField] private string missedMessage;
    
    public static AccuracyTextManager Instance { get; private set; }
    
    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    public void SetAccuracyText(Target target, PressedAccuracy accuracy)
    {
        TextMeshProUGUI targetText;
        switch (target)
        {
            case Target.Top:
                targetText = topAccuracyText;
                break;
            case Target.Right:
                targetText = rightAccuracyText;
                break;
            case Target.Left:
                targetText = leftAccuracyText;
                break;
            case Target.Bottom:
                targetText = bottomAccuracyText;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (accuracy)
        {
            case PressedAccuracy.Perfect:
                targetText.text = perfectMessage;
                break;
            case PressedAccuracy.Good:
                targetText.text = goodMessage;
                break;
            case PressedAccuracy.Ok:
                targetText.text = okMessage;
                break;
            case PressedAccuracy.Bad:
                targetText.text = badMessage;
                break;
            case PressedAccuracy.Missed:
                targetText.text = missedMessage;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
