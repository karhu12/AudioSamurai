using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : Singleton<ModeManager>
{

    private const string SD_MODE_PREF = "SuddenDeathMode";
    private const string NF_MODE_PREF = "NoFailMode";

    public const int NO_FAIL_MOD = 2;
    public const int SUDDEN_DEATH_MOD = 1;
    public const int NORMAL_MOD = 0;

    /* 
     * Returns mode multiplier 
     * If   Normal-mode = NORMAL_MOD
     *      Sudden death -mode = SUDDENT_DEATH_MOD
     *      No Fail -mode = NO_FAIL_MOD
     */

    public int GetMode()
    {
        if (PlayerPrefs.GetInt(SD_MODE_PREF) == 1)
            return SUDDEN_DEATH_MOD;

        else if (PlayerPrefs.GetInt(NF_MODE_PREF) == 1)
            return NO_FAIL_MOD;

        return NORMAL_MOD;
    }
}
