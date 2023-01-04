using System.Collections.Generic;
using UnityEngine;

public static class Convolution
{
    public static float[] Convolve1D(float[] f, float[] g)
    {
        List<float> result = new List<float>();
        int n = f.Length;
        int m = g.Length;
        for(int i = 0; i < n+m-1; i++)
        {
            float sum = 0;
            for(int j = Mathf.Max(0,i-(m-1)); j < Mathf.Min(n-1,i); j++)
            {
                    sum += f[i-j] * g[m-1 - j];
            }
            result.Add(sum);
        }
        return result.ToArray();
    }
}
