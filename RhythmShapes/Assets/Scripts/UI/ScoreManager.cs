using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using shape;
using ui;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private int perfectPoints = 100;
    [SerializeField] private int goodPoints = 60;
    [SerializeField] private int okPoints = 30;
    [SerializeField] private int badPoints = 10;
    [SerializeField] private int missPoints = 0;

    public int PerfectCounter { get; private set; }
    public int GoodCounter { get; private set; }
    public int OkCounter { get; private set; }
    public int BadCounter { get; private set; }
    public int MissCounter { get; private set; }

    public int Score { get; private set; }
    private float currrentAccuracy;
    private int currentNumberOfShape = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
        else
            Instance = this;
    }

    public void OnInputValidated(Target target, PressedAccuracy accuracy)
    {
        int amount = 0;
        switch (accuracy)
        {
            case PressedAccuracy.Perfect:
                amount = perfectPoints;
                PerfectCounter++;
                break;
            case PressedAccuracy.Good:
                amount = goodPoints;
                GoodCounter++;
                break;
            case PressedAccuracy.Ok:
                amount = okPoints;
                OkCounter++;
                break;
            case PressedAccuracy.Bad:
                amount = badPoints;
                BadCounter++;
                break;
            case PressedAccuracy.Missed:
                amount = missPoints;
                MissCounter++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
                amount = 0;
        }

        Score += amount;
        // currentNumberOfShape++;
        // currrentAccuracy = (500*perfectCounter + 200*goodCounter + 75*okCounter + 40*goodCounter + 10*badCounter)/((float)500*currentNumberOfShape);
    }

    public int GetScore()
    {
        return Score;
    }
    
    public float GetAccuracy()
    {
        return currrentAccuracy;
    }
    
    public void ResetScoreVariables()
    {
        PerfectCounter = 0;
        GoodCounter = 0;
        OkCounter = 0;
        BadCounter = 0;
        MissCounter = 0;
        Score = 0;
        currrentAccuracy = 0;
    }
}



