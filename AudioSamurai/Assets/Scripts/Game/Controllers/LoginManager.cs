using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : Singleton<LoginManager>
{
    public const string LOGIN_PREF = "login";
    public const string USERNAME_PREF = "username";

    public const int LOGGED_OUT = 0;
    public const int LOGGED_IN = 1;
    public const int OFFLINE = 2;

    public int GetLoginStatus()
    {
        if (PlayerPrefs.GetInt(LOGIN_PREF) == LOGGED_IN)
            return LOGGED_IN;

        else if (PlayerPrefs.GetInt(LOGIN_PREF) == OFFLINE)
            return OFFLINE;

        return LOGGED_OUT;
    }

    public void LogIn(int loginStatus, string username)
    {
        PlayerPrefs.SetInt(LOGIN_PREF, loginStatus);
        PlayerPrefs.SetString(USERNAME_PREF, username);
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
    }

    public void LogOut()
    {
        PlayerPrefs.SetInt(LOGIN_PREF, LOGGED_OUT);
        PlayerPrefs.SetString(USERNAME_PREF, "");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Login);
    }

}
