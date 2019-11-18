using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;

    public GameObject mainMenuUI;

    

    // Update is called once per frame
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

    public void LoadMenu()
    {
        mainMenuUI.SetActive(true);
        onMainMenu = true;
    }
     
    public void LoadSettings()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.OptionsMenu);
        Debug.Log("loading...");
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        } else
        {
            Application.Quit();
        }
            
    }   
}
