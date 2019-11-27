using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : Singleton<ModeManager>
{

    private const string SD_MODE_PREF = "SuddenDeathMode";
    private const string NF_MODE_PREF = "NoFailMode";

    /* 
     * Returns mode multiplier 
     * If   Normal-mode = 0
     *      Sudden death -mode = 1
     *      No Fail -mode = 2
     */

    public int GetMode()
    {

        if (PlayerPrefs.GetInt(SD_MODE_PREF) == 1)
            return 1;

        else if (PlayerPrefs.GetInt(NF_MODE_PREF) == 1)
            return 2;

        return 0;
    }
}
