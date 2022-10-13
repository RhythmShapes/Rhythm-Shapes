using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Square : Shape
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Color = SpriteRenderer.color;
        ShapeType = ShapeType.Square;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    protected override void UpdateColor()
    {
        if (Color != SpriteRenderer.color)
        {
            SpriteRenderer.color = Color;
        }
    }
}