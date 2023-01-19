using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransitionCurves
{
    public static Vector2 CubicBezier(float x1, float x2, float y1, float y2, float t)
    {
        Vector2 x= new Vector2(x1,x2);
        Vector2 y= new Vector2(y1,y2);
        return 3 * (1 - t) * (1 - t) * t * x + 3 * (1 - t) * t * t * y + t * t * t * new Vector2(1f,1f);
    } 
}
