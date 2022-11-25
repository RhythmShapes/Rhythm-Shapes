using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SetupDatapath : MonoBehaviour
{
    private bool setupNeeded = true;
    private void Awake()
    {
        if(Directory.Exists(Path.Combine(Application.persistentDataPath,"Levels","LevelTest")))
        {
            setupNeeded = false;
        }

        if (setupNeeded)
        {
            FileUtil.CopyFileOrDirectory(Path.Combine(Application.dataPath,"Resources","Levels","LevelTest"), Path.Combine(Application.persistentDataPath,"Levels","LevelTest"));
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
