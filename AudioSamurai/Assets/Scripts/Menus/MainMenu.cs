using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;

    static bool notFirstTime = false;

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
        SceneManager.LoadScene(sceneName: "Credits");
        //CameraController.Instance.SetCameraToState(CameraController.CameraState.Quit);
    }

}
