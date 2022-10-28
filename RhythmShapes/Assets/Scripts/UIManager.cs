using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redText;
    [SerializeField] private TextMeshProUGUI blueText;
    [SerializeField] private TextMeshProUGUI greenText;
    [SerializeField] private TextMeshProUGUI yellowText;

    private float timerRed;
    private float timerBlue;
    private float timerGreen;
    private float timerYellow;
    
    public static UIManager Instance { get; private set; }


    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRedTextGood()
    {
        redText.text = "GOOOD";
    }
    
    public void SetRedTextMiss()
    {
        redText.text = "MISS";
    }
    
    public void SetBlueTextGood()
    {
        blueText.text = "GOOOD";
    }
    
    public void SetBlueTextMiss()
    {
        blueText.text = "MISS";
    }
    
    public void SetGreenTextGood()
    {
        greenText.text = "GOOOD";
    }
    
    public void SetGreenTextMiss()
    {
        greenText.text = "MISS";
    }
    
    public void SetYellowTextGood()
    {
        yellowText.text = "GOOOD";
    }
    
    public void SetYellowTextMiss()
    {
        yellowText.text = "MISS";
    }
}
