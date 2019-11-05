using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;
    public static bool onPause = false;

    public GameObject mainMenuUI;
    public GameObject pauseUI;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) && !onMainMenu)
        {
            if (onPause)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void OnStart()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
    }

    public void CheckMenuStatus()
    {
        if (onMainMenu)
        {
            CloseMainMenu();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        onMainMenu = true;
    }

    void CloseMainMenu()
    {
        mainMenuUI.SetActive(false);
        onMainMenu = false;
    }

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

    public void LoadMenu()
    {
        pauseUI.SetActive(false);
        mainMenuUI.SetActive(true);
        onMainMenu = true;
        onPause = false;
    }

    public void LoadSettings()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.OptionsMenu);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
    }   
}
