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
            FindObjectOfType<AudioManager>().Play("Fail");
            //audioSource.PlayOneShot(failMusic);
        } else {
            FindObjectOfType<AudioManager>().Stop("Fail");
            //audioSource.Stop();
        }
    }

    public void OnRetry() {
        FindObjectOfType<AudioManager>().Play("Click");
        GameController.Instance.Retry();
        ToggleFailMusic(false);
    }

    public void OnBackToMenu() {
        FindObjectOfType<AudioManager>().Play("Click");
        GameController.Instance.QuitGame();
        ToggleFailMusic(false);
    }
}
