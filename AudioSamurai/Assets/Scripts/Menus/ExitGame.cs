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
            if (Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }

            else
            {
                Application.Quit();
            }
        }

        else
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0)
            {
                if (Application.isEditor)
                {
                    EditorApplication.isPlaying = false;
                }

                else
                {
                    Application.Quit();
                }
            }
        }
    }
}
