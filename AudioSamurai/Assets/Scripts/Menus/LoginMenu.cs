using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenu : MonoBehaviour
{
    public static bool onLogin = true;
    public static bool onSignin = false;

    public GameObject LogInUI;
    public GameObject SignInUI;
    public GameObject SuccessfulSign;
    public GameObject OfflineButton;

    private string username, password;
    void Start()
    {

        CameraController.Instance.SetCameraToState(CameraController.CameraState.Login);

        /* Checks if already logged in */
        switch (LoginManager.Instance.GetLoginStatus())
        {
            case LoginManager.LOGGED_IN:
                onLogin = false;
                CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
                break;

            case LoginManager.OFFLINE:
                onLogin = false;
                CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
                break;
                
        }
    }


    public void TryToLogin()
    {
        /*
         * Todo: Check username and password from database
         *  if successful SuccesfullLogin();
         *  else FailedToLogin();
         */
        SuccesfullLogin();
    }

    public void TryToSignIn()
    {
        /*
         * Todo: Check username and password from database
         *  if username is already taken save FailedToSignIn();
         *  else SuccesfullSignIn(); 
         */
        SuccesfullSignIn();
    }

    public void SuccesfullLogin()
    {
        LoginManager.Instance.LogIn(LoginManager.LOGGED_IN,username);
    }

    public void FailedToLogin()
    {
        //complain about Login
    }

    public void SuccesfullSignIn()
    {
        onLogin = false;
        SignInUI.SetActive(false);
        SuccessfulSign.SetActive(true);
        OfflineButton.SetActive(false);
        //Save username and password
    }

    public void FailedToSignIn()
    {
        //complain about Sign in
    }

    public void PlayOffline()
    {
        LoginManager.Instance.LogIn(LoginManager.OFFLINE, "");
        if (onSignin)
            SwapView();
    }

    public void SwapView()
    {
        SuccessfulSign.SetActive(false);
        if (onLogin)
        {
            LogInUI.SetActive(false);
            onLogin = false;
            SignInUI.SetActive(true);
            onSignin = true;
        }
        else
        {
            SignInUI.SetActive(false);
            onSignin = false;
            LogInUI.SetActive(true);
            onLogin = true;
        }
        OfflineButton.SetActive(true);
    }


    public void ReadUsername(string text)
    {
        username = text;
    }

    public void ReadPassword(string text)
    {
        password = text;
    }
}
