using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JIN2023 : MonoBehaviour
{
    private string code = "JIN2023";
    private string completedCode = "";
    private string oldInput;
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private SpriteRenderer square;
    [SerializeField] private SpriteRenderer diamond;

    [SerializeField] private Sprite base_circle;
    [SerializeField] private Sprite base_square;
    [SerializeField] private Sprite base_diamond;

    [SerializeField] private Sprite jin;
    [SerializeField] private Sprite ensiie;
    [SerializeField] private Sprite tsp;
    private void Start()
    {
        diamond.sprite = base_diamond;
        circle.sprite = base_circle;
        square.sprite = base_square;
    }
    
    public void CheatCode(string input)
    {
        if(input == "J" && input != oldInput && completedCode == "")
        {
            completedCode = "J";
        }
        else if(input == "I" && input != oldInput && completedCode == "J")
        {
            completedCode = "JI";
        }
        else if (input == "N" && input != oldInput && completedCode == "JI")
        {
            completedCode = "JIN";
        }
        else if (input == "2" && input != oldInput && completedCode == "JIN")
        {
            completedCode = "JIN2";
        }
        else if(input == "0" && input != oldInput && completedCode == "JIN2")
        {
            completedCode = "JIN20";
        }
        else if (input == "2" && input != oldInput && completedCode == "JIN20")
        {
            completedCode = "JIN202";
        }
        else if (input == "3" && input != oldInput && completedCode == "JIN202")
        {
            completedCode = "JIN2023";
            ActivateJIN();
        }
        else
        {
            oldInput = "";
            completedCode = "";
        }
    }

    private void ActivateJIN()
    {
        diamond.sprite = jin;
        circle.sprite = ensiie;
        square.sprite = tsp;
        Debug.Log("CheatCodeActivated, welcome to JIN2023");
    }

    private void Update()
    {
        if(Input.inputString.Length ==1)
        {
            CheatCode(Input.inputString.ToUpper());
        }
        if(code == completedCode)
        {
            ActivateJIN();
            code = "-165431dza";
        }
    }
}
