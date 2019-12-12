using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public enum CameraState
    {
        Menu,
        SongSelection,
        Quit,
        OptionsMenu,
        Game,   
        GameResult,
        FailMenu,
        Login,
        HelpMenu,
        Highscore,
        Pause
    }

    public CameraState cameraState = CameraState.Menu;

    private Animator animator;
    public Animator Animator
    {
        get
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            return animator;
        }
    }

    /*
     * Sets the main camera to the given camera state which triggers the main camera movement using state driven camera.
     */
    public bool SetCameraToState(CameraState state)
    {
        if (cameraState != state)
        {
            cameraState = state;
            foreach (var cameraState in Enum.GetValues(typeof(CameraState)))
            {
                Animator.ResetTrigger(cameraState.ToString());
            }
            Instance.Animator.SetTrigger(state.ToString());
            return true;
        }
        return false;
    }
}
