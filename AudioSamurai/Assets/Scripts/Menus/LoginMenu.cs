using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginMenu : MonoBehaviour
{
    public static bool onLogin = true;
    public static bool onSignin = false;

    public GameObject LogInUI;
    public GameObject SignInUI;
    public GameObject SuccessfulSign;
    public GameObject OfflineButton;
    public GameObject ErrorMessageObj;
    public TextMeshProUGUI ErrorMessage;
    public Animator animator;

    private string username, password;
    void Start()
    {
        animator = ErrorMessage.GetComponent<Animator>();
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Login);
        Mongo.Instance.Init();
        //ToDo: Login music

        /* Checks if already logged in */
        switch (LoginManager.Instance.GetLoginStatus())
        {
            case LoginManager.LOGGED_IN:
                Mongo.Instance.SetDataIfLogin(PlayerPrefs.GetString(LoginManager.USERNAME_PREF));
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
        SetCredentialValues(LogInUI);
        Mongo.Instance.GetPlayerByCredentials(username, password);
        if(Mongo.Instance.LoginSuccess)
        {
            SuccessfulLogin();
        }
        else
        {
            FailedToLogin();
        }
    }

    public async void TryToSignIn()
    { 
        SetCredentialValues(SignInUI);
        if (!username.Equals("") && !password.Equals(""))
        {
            bool result = await Mongo.Instance.CheckIfAvailable(username);
            if (result)
            {
                Mongo.Instance.RegisterNewPlayer(username, password);
                SuccessfulSignIn();
            }
            else
            {
                FailedToSignIn();
            }
        }
    }

    public void SuccessfulLogin()
    {
        LoginManager.Instance.LogIn(LoginManager.LOGGED_IN,username);
        Clear(LogInUI);
    }

    public void SuccessfulSignIn()
    {
        onLogin = false;
        SignInUI.SetActive(false);
        SuccessfulSign.SetActive(true);
        OfflineButton.SetActive(false);
        Clear(SignInUI);
    }

    public void FailedToLogin()
    {
        StartCoroutine(ShowErrorMessage("Login failed. Username or password was invalid."));
    }

    public void FailedToSignIn()
    {
        StartCoroutine(ShowErrorMessage("Sign in failed. Username is already taken."));
    }

    public IEnumerator ShowErrorMessage(String message)
    {
        ErrorMessageObj.SetActive(true);
        ErrorMessage.text = message;
        animator.SetTrigger("Visible");
        yield return new WaitForSeconds(3);
        animator.SetTrigger("Vanish");
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
            Clear(LogInUI);
            LogInUI.SetActive(false);
            onLogin = false;
            SignInUI.SetActive(true);
            onSignin = true;
        }
        else
        {
            Clear(SignInUI);
            SignInUI.SetActive(false);
            onSignin = false;
            LogInUI.SetActive(true);
            onLogin = true;
        }
        OfflineButton.SetActive(true);
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
