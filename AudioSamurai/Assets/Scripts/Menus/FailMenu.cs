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
