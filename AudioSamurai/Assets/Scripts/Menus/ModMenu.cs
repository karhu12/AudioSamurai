using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModMenu : Singleton<ModMenu>
{

    public Toggle normalModeToggle;
    public Toggle sdModeToggle;
    public Toggle nfModeToggle;

    private int normalMode;
    private int sdMode;
    private int nfMode;

    private const string NORLMA_MODE_PREF = "NormalMode";
    private const string SD_MODE_PREF = "SuddenDeathMode";
    private const string NF_MODE_PREF = "NoFailMode";

    void Awake()
    {

        normalMode = PlayerPrefs.GetInt(NORLMA_MODE_PREF);
        sdMode = PlayerPrefs.GetInt(SD_MODE_PREF);
        nfMode = PlayerPrefs.GetInt(NF_MODE_PREF);

        if (sdMode == 1) {
            normalModeToggle.isOn = false;
            sdModeToggle.isOn = true;
            nfModeToggle.isOn = false;
        }else if (nfMode == 1) {
            normalModeToggle.isOn = false;
            sdModeToggle.isOn = false;
            nfModeToggle.isOn = true;
        }else{
            normalModeToggle.isOn = true;
            sdModeToggle.isOn = false;
            nfModeToggle.isOn = false;
        }
    }

    public void ToggleView()
    {

    }

    public int ReturnMode()
    {
        if (sdMode == 1)
            return 100;

        else if (nfMode == 1)
            return 0;
   
        else return 1;
    }

    public void NormalModeToggle(bool modeState)
    {
        if (modeState == false)
        {
            PlayerPrefs.SetInt(NORLMA_MODE_PREF, 0);
        }

        else
        {
            PlayerPrefs.SetInt(NORLMA_MODE_PREF, 1);
        }
    }

    public void SDModeToggle(bool modeState)
    {
        if (modeState == false)
        {
            PlayerPrefs.SetInt(SD_MODE_PREF, 0);
        }

        else
        {
            PlayerPrefs.SetInt(SD_MODE_PREF, 1);
        }
    }

    public void NFModeToggle(bool modeState)
    {
        if (modeState == false)
        {
            PlayerPrefs.SetInt(NF_MODE_PREF, 0);
        }

        else
        {
            PlayerPrefs.SetInt(NF_MODE_PREF, 1);
        }
    }
}
