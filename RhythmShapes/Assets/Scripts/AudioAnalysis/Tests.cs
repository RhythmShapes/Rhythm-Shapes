using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float[] a = { 1, 2, 3 };
        float[] b = { 4, 5, 6 };
        float[] c = Convolution.Convolve1D(a,b);
        Debug.Log(c);
        for(int i = 0; i < c.Length; i++)
        {
            Debug.Log(c[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
