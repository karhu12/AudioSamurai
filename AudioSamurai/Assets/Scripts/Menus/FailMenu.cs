using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip failMusic;

    /* Constants */
    public const string TAG = "FailMenu";

    public void ToggleFailMusic(bool on) {
        if (on) {
            audioSource.PlayOneShot(failMusic);
        } else {
            audioSource.Stop();
        }
    }

    public void OnRetry() {
        GameController.Instance.Retry();
        ToggleFailMusic(false);
    }

    public void OnBackToMenu() {
        GameController.Instance.QuitGame();
        ToggleFailMusic(false);
    }
}
