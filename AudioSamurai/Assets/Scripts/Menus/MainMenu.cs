using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;
    
    public GameObject mainMenuUI;

    private void Start()
    {
            FindObjectOfType<AudioManager>().Play("MenuMusic");
    }
    // Update is called once per frame
    public void OnStart()
    {
        FindObjectOfType<AudioManager>().Play("Click");
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
        FindObjectOfType<AudioManager>().Pause("MenuMusic");
    }

    public void LoadMenu()
    {
        mainMenuUI.SetActive(true);
        onMainMenu = true;
    }
     
    public void LoadSettings()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.OptionsMenu);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            FindObjectOfType<AudioManager>().Play("Click");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            FindObjectOfType<AudioManager>().Play("Click");
            Application.Quit();
        #endif
    }   
}
