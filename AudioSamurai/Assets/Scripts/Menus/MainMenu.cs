using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static bool onMainMenu = true;
    
    public GameObject mainMenuUI;
    public TextMeshProUGUI LoginButtonText;
    public TextMeshProUGUI UsernameLabel;

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MenuMusic");
    }

    public void RefreshPlayer() {
        if (LoginManager.Instance.GetLoginStatus() == LoginManager.LOGGED_IN) {
            LoginButtonText.text = "Log out";
            UsernameLabel.text = "Playing as " + LoginManager.Instance.GetUsername();
        }
        else {
            UsernameLabel.text = "Playing Offline";
            LoginButtonText.text = "Log in";
        }
    }

    // Update is called once per frame
    public void OnStart()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
    }
     
    public void LoadHelpMenu()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.HelpMenu);
    }

    public void LoadSettings()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.OptionsMenu);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(sceneName: "Credits");
    }

    public void LogOut() {
        LoginManager.Instance.LogOut();
    }
}
