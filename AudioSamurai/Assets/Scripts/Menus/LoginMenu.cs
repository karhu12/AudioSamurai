using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    void Start()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Login);
        Mongo.Instance.Init();
        //ToDo: Login music

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
        SetCredentialValues(LogInUI);
        Mongo.Instance.GetPlayerByCredentials(username, password);
        if(Mongo.Instance.LoginSuccess)
        {
            SuccessfulLogin();
            Debug.Log("Logged in");
        }
        else
        {
            Debug.Log("Error while logging. Try again.");
        }
    }

    public void TryToSignIn()
    {
        SetCredentialValues(SignInUI);
        if (!username.Equals("") && !password.Equals(""))
        {
            if (Mongo.Instance.CheckIfAvailable(username))
            {
                Mongo.Instance.RegisterNewPlayer(username, password);
                SuccessfulSignIn();
                Debug.Log("User created.");
            }
            else
            {
                FailedToSignIn();
                Debug.Log("Username taken. Try again.");
            }
        }
    }

    public void SuccessfulLogin()
    {
        PlayerPrefs.SetInt(LOGIN_PREF, 1);
        PlayerPrefs.SetString(USERNAME_PREF, username);
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
        Clear(LogInUI);
    }

    public void FailedToLogin()
    {
        //complain about Login
    }

    public void SuccessfulSignIn()
    {
        //Save username and password
        Clear(SignInUI);
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

    public void SetCredentialValues(GameObject gameObject)
    {
        Transform pass = gameObject.transform.Find("Password").Find("InputFieldPass");
        Transform user = gameObject.transform.Find("User").Find("InputFieldUser");
        password = pass.GetComponent<InputField>().text;
        username = user.GetComponent<InputField>().text;
    }

    public void Clear(GameObject gameObject)
    {
        Transform pass = gameObject.transform.Find("Password").Find("InputFieldPass");
        Transform user = gameObject.transform.Find("User").Find("InputFieldUser");
        pass.GetComponent<InputField>().text = "";
        user.GetComponent<InputField>().text = "";
        password = "";
        username = "";   
    }
}
