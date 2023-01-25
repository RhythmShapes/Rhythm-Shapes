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

    public int Score { get; private set; } = 0;
    public int Combo { get; private set; } = 0;
    
    public int BestCombo { get; private set; } = 0;
    
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
                Combo++;
                if (Combo > BestCombo) BestCombo++;
                amount = perfectPoints * GetComboValue();
                PerfectCounter++;
                break;
            case PressedAccuracy.Good:
                Combo++;
                if (Combo > BestCombo) BestCombo++;
                amount = goodPoints * GetComboValue();
                GoodCounter++;
                break;
            case PressedAccuracy.Ok:
                Combo++;
                if (Combo > BestCombo) BestCombo++;
                amount = okPoints * GetComboValue();
                OkCounter++;
                break;
            case PressedAccuracy.Bad:
                Combo = 0;
                amount = badPoints;
                BadCounter++;
                break;
            case PressedAccuracy.Missed:
                Combo = 0;
                amount = missPoints;
                MissCounter++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Score += amount;
        currentNumberOfShape++;
        currrentAccuracy = (400*PerfectCounter + 200*GoodCounter + 100*OkCounter + 50*GoodCounter + 25*BadCounter)/((float)400*currentNumberOfShape);
    }

    private int GetComboValue()
    {
        return Combo > 0 ? Combo : 1;
    }

    public int GetScore()
    {
        return Score;
    }
    
    public int GetBestCombo()
    {
        return BestCombo;
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
        Combo = 0;
        currentNumberOfShape = 0;
        currrentAccuracy = 0;
        BestCombo = 0;
    }

    public void PrintScoreDebug()
    {
        // Debug.Log("Score : "+ Score + ",PerfectCounter : "+ PerfectCounter+ ",GoodCounter : "+ GoodCounter+ ",OkCounter : "+ OkCounter+ ",BadCounter : "+ BadCounter+ ",MissCounter : "+ MissCounter);
    }
}



