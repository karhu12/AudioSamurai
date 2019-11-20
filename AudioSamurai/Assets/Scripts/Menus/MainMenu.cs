using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;

    static bool notFirstTime = false;

    public GameObject mainMenuUI;

    AudioSource audioSource;



    private void Start()
    {
        if (notFirstTime == false)
        {
            Debug.Log(notFirstTime);
            FindObjectOfType<AudioManager>().Play("MenuMusic");
            notFirstTime = !notFirstTime;
        }
        else
        {
            
        }
        
    }

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
