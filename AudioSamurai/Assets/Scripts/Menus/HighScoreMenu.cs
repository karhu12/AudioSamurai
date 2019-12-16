using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class HighScoreMenu : MonoBehaviour
{
    public Text SongNameLabel;
    public GameObject songPreFab;
    public ScrollRect scrollView;
    public RectTransform content;
    private string songname;

    //Set camera to leaderboards state and insert leaderboards to scrollview for display.
    public void UpdateView()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Highscore);
        ShowLeaderboards(LeaderBoardManager.Instance.Leaderboards, LeaderBoardManager.Instance.MapName);
    }
    public void GoBack()
    {
        CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
    }

    //Loop through leaderboard list and remove all empty entries. Set every list item into one leaderboard item inside scrollview.
    public void ShowLeaderboards(List<LeaderBoardItem> list, string mapName)
    {
        songname = mapName;
        SongNameLabel.text = songname;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Score == 0)
                break;
            else
            {
                GameObject gameObject = content.gameObject;
                LeaderBoardView view = new LeaderBoardView(ref content, songPreFab);
                Transform lbRank = view.gameObject.transform.GetChild(0);
                Transform lbUser = view.gameObject.transform.GetChild(1);
                Transform lbScore = view.gameObject.transform.GetChild(2);
                lbRank.GetComponent<Text>().text = (i + 1).ToString();
                lbUser.GetComponent<Text>().text = list[i].Name;
                lbScore.GetComponent<Text>().text = Format(list[i].Score);
                view.gameObject.transform.SetParent(gameObject.transform, false);
                view.gameObject.SetActive(true);
            }
        }
    }

    //Format the score
    public static string Format(int score)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return score.ToString("#,0", nfi);
    }
}

//LeaderboardItem for scrollview
public class LeaderBoardView
{
    public GameObject gameObject;
    public LeaderBoardView(ref RectTransform content, GameObject prefab)
    {
        gameObject = MonoBehaviour.Instantiate(prefab);
        gameObject.transform.SetParent(content, false);
    }
}