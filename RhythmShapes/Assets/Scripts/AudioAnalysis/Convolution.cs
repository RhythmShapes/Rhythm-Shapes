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
            int start = Mathf.Max(0, k + 1 - m);
            int end = Mathf.Min(n, k);
            for (int i = start; i<end; i++)
            {
                if(k > 2000 && k<2020)
                {
                    //Debug.Log("f: " + f[i] + " g:" + g[k - i] + " f*g:" + f[i]*g[k-i]);
                }
                sum += f[i] * g[k - i];
            }
            result.Add(sum);
        }
        return result.ToArray();
    }
}