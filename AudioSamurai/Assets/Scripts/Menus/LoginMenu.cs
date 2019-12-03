using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenu : MonoBehaviour
{
    private const string LOGIN_PREF = "login";
    private const string USERNAME_PREF = "username";

    public const int LOGGED_IN = 1;

    public static bool onLogin = true;
    public static bool onSignin = false;

    public GameObject LogInUI;
    public GameObject SignInUI;

    private string username, password;
    //private Mongo mongo;
    void Start()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Login);
        //mongo = new Mongo();
        Mongo.Instance.Init();
        //ToDo: Login music

        //PlayerPrefs.SetInt(LOGIN_PREF, 0);
        /* Checks if already logged in */
        if (PlayerPrefs.GetInt(LOGIN_PREF) == LOGGED_IN)
        {
            Mongo.Instance.SetDataIfLogin(PlayerPrefs.GetString(USERNAME_PREF));
            onLogin = false;
            CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
        }
    }


    public void TryToLogin()
    {
        /*
         * Todo: Check username and password from database
         *  if successful SuccesfullLogin();
         *  else FailedToLogin();
         */
        Mongo.Instance.GetPlayerByCredentials(username, password);
        if(Mongo.Instance.Success)
        {
            SuccessfulLogin();
        }
    }

    public void TryToSignIn()
    {
        /*
         * Todo: Check username and password from database
         *  if username is already taken save FailedToSignIn();
         *  else SuccesfullSignIn(); 
         */
        if (Mongo.Instance.CheckIfAvailable(username))
        {
            Mongo.Instance.RegisterNewPlayer(username, password);
            SuccessfulSignIn();
        }
        else
        {
            FailedToSignIn();
        }
    }

    public void SuccessfulLogin()
    {
        PlayerPrefs.SetInt(LOGIN_PREF, 1);
        PlayerPrefs.SetString(USERNAME_PREF, username);
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
    }

    public void FailedToLogin()
    {
        //complain about Login
    }

    public void SuccessfulSignIn()
    {
        //Save username and password
    }

    public void FailedToSignIn()
    {
        //complain about Sign in
    }

    public void SwapView()
    {
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
