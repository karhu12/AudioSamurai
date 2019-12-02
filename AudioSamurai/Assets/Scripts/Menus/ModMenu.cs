using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModMenu : MonoBehaviour
{
    public GameObject ToggleGroup;
    public Toggle sdModeToggle;
    public Toggle nfModeToggle;
   
    private int sdMode;
    private int nfMode;

    private const string SD_MODE_PREF = "SuddenDeathMode";
    private const string NF_MODE_PREF = "NoFailMode";

    void Awake()
    {
        sdMode = PlayerPrefs.GetInt(SD_MODE_PREF);
        nfMode = PlayerPrefs.GetInt(NF_MODE_PREF);

        if (sdMode == 1) {
            sdModeToggle.isOn = true;
            nfModeToggle.isOn = false;
        }else if (nfMode == 1) {
            sdModeToggle.isOn = false;
            nfModeToggle.isOn = true;
        }else{
            sdModeToggle.isOn = false;
            nfModeToggle.isOn = false;
        }
    }

    public void ToggleView()
    {
        if (ToggleGroup != null)
        {
            Animator animator = ToggleGroup.GetComponent<Animator>();
            if (animator != null)
            {
                bool isOpen = animator.GetBool("open");
                animator.SetBool("open", !isOpen);
            }
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
