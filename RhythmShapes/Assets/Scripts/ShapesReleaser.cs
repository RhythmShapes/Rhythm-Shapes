using System;
using System.Collections;
using System.Collections.Generic;
using shape;
using UnityEngine;

public class ShapesReleaser : MonoBehaviour
{
    public static ShapesReleaser Instance { get; private set; }

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
    
    public void ReleaseIfOutOfTime()
    {
        if (GameplayManager.Instance._topTargetQueue.Count == 0 && 
            GameplayManager.Instance._leftTargetQueue.Count == 0 && 
            GameplayManager.Instance._rightTargetQueue.Count == 0 &&
            GameplayManager.Instance._bottomTargetQueue.Count == 0)
        {
            return;
        }
        else
        {
            float topTime = 10000000;
            float leftTime= 10000000;
            float rightTime = 10000000;
            float bottomTime = 10000000;

            if (GameplayManager.Instance._topTargetQueue.Count != 0)
            {
                topTime = GameplayManager.Instance._topTargetQueue.Peek().TimeToPress;
            }
        
            if (GameplayManager.Instance._leftTargetQueue.Count != 0)
            {
                leftTime = GameplayManager.Instance._leftTargetQueue.Peek().TimeToPress;
            }
            if (GameplayManager.Instance._rightTargetQueue.Count != 0)
            {
                rightTime = GameplayManager.Instance._rightTargetQueue.Peek().TimeToPress;
            }
            if (GameplayManager.Instance._bottomTargetQueue.Count != 0)
            {
                bottomTime = GameplayManager.Instance._bottomTargetQueue.Peek().TimeToPress;
            }

            float min = Mathf.Min(topTime, leftTime, rightTime, bottomTime);
            // Debug.Log("ReleaseIfOutOfTime -> Min : " + min);
            if (min == topTime && GameplayManager.Instance.globalTime >  topTime + GameplayManager.Instance.goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> topTime : " + min);
                UIManager.Instance.SetBlueTextMiss();
                ShapeFactory.Instance.Release(GameplayManager.Instance._topTargetQueue.Dequeue());
            }
            else if (min == leftTime && GameplayManager.Instance.globalTime >  leftTime + GameplayManager.Instance.goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> leftTime : " + min);
                UIManager.Instance.SetGreenTextMiss();
                ShapeFactory.Instance.Release(GameplayManager.Instance._leftTargetQueue.Dequeue());
            }
            else if (min == rightTime && GameplayManager.Instance.globalTime >  rightTime + GameplayManager.Instance.goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> rightTime : " + min);
                UIManager.Instance.SetRedTextMiss();
                ShapeFactory.Instance.Release(GameplayManager.Instance._rightTargetQueue.Dequeue());
            }
            else if (min == bottomTime && GameplayManager.Instance.globalTime >  bottomTime + GameplayManager.Instance.goodWindow)
            {
                // Debug.Log("ReleaseIfOutOfTime -> bottomTime : " + min);
                UIManager.Instance.SetYellowTextMiss();
                ShapeFactory.Instance.Release(GameplayManager.Instance._bottomTargetQueue.Dequeue());
            }
            // else
            // {
            //     Debug.Log("Not supposed to happened");
            // }
        }
    }
}
