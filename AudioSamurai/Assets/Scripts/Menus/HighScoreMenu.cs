using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HighScoreMenu : MonoBehaviour
{
    public Text SongNameLabel;
    public GameObject songPreFab;

    public string songname;
    public string rank;
    public string username;
    public string highscore;


    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }


    public void Refresh()
    {

    }

    public void UpdateView()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Highscore);
    }
    public void GoBack()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
    }
}
