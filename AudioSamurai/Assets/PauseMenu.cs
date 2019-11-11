using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool onPause = false;

    public GameObject pauseUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //ResumeButton OnClick
    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        onPause = false;
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        onPause = true;
    }

    //MenuButton OnClick
    public void LoadMainMenu()
    {
        pauseUI.SetActive(false);
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
        Debug.Log("Loading main menu...");
    }
}
