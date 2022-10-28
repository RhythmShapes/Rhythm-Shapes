using System;
using System.Collections;
using System.Collections.Generic;
using shape;
using UnityEngine;

public class Validation : MonoBehaviour
{
    
    public static Validation Instance { get; private set; }


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

    public bool TestInput(Queue<Shape> targetQueue)
    {
        Shape currentShape = targetQueue.Peek();
        // _tapTime = _audioSource.time;
        float _tapTime = GameplayManager.Instance.globalTime;
        if (_tapTime > currentShape.TimeToPress - GameplayManager.Instance.goodWindow && _tapTime < currentShape.TimeToPress + GameplayManager.Instance.goodWindow)
        {
                
            Debug.Log("Top : GOOOOOOOD, tapTime : " + _tapTime + ", TimeToPress : " + currentShape.TimeToPress);
            ShapeFactory.Instance.Release(targetQueue.Dequeue());
            return true;
        }

        return false;
    }
}
