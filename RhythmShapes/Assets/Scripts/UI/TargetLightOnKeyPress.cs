using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using models;
using ui;
using shape;

public class TargetLightOnKeyPress : MonoBehaviour
{
    [SerializeField] public SpriteRenderer topTarget;
    [SerializeField] public SpriteRenderer bottomTarget;
    [SerializeField] public SpriteRenderer leftTarget;
    [SerializeField] public SpriteRenderer rightTarget;
    private Color topColor;
    private Color bottomColor;
    private Color leftColor;
    private Color rightColor;

    void Start()
    {
        topColor = topTarget.color;
        bottomColor = bottomTarget.color;
        leftColor = leftTarget.color;
        rightColor = rightTarget.color;
    }

    public void On(Target target) {
        if(target == null)
        {
            return;
        }
        else if(target == Target.Top)
        {
            topTarget.color = Color.white;
        }
        else if(target== Target.Bottom)
        {
            bottomTarget.color = Color.white;
        }
        else if(target == Target.Left)
        {
            leftTarget.color = Color.white;
        }
        else
        {
            rightTarget.color = Color.white;
        }
    }

    public void Off(Target target)
    {
        if (target == null)
        {
            return;
        }
        else if (target == Target.Top)
        {
            topTarget.color = topColor;
        }
        else if (target == Target.Bottom)
        {
            bottomTarget.color = bottomColor;
        }
        else if (target == Target.Left)
        {
            leftTarget.color = leftColor;
        }
        else
        {
            rightTarget.color = rightColor;
        }
    }
}
