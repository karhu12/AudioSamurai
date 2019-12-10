using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExitGame : MonoBehaviour
{

    private float countdownTime = 10.0f;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0)
            {
                #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }
    }
}
