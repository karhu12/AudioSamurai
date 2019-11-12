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
        if (GameController.Instance.Unpause())
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1f;
            onPause = false;
        }
    }

    public void Pause()
    {
        if (GameController.Instance.Pause())
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0f;
            onPause = true;
        }
    }

    //MenuButton OnClick
    public void LoadMainMenu()
    {
        Resume();
        GameController.Instance.QuitGame();
        Debug.Log("Loading main menu...");
    }
}
