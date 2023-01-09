using System.Collections.Generic;
using UnityEngine;

public static class Convolution
{
    public static float[] Convolve1D(float[] f, float[] g)
    {
        List<float> result = new List<float>();
        int n = f.Length;
        int m = g.Length;
        for(int k = 0; k < n+m-1; k++)
        {
            float sum = 0;
            for (int i = Mathf.Max(0,k+1-m); i<Mathf.Min(m,k+1); i++)
            {
                sum += f[i] * g[k - i];
            }
            result.Add(sum);
        }
        return result.ToArray();
    }
}