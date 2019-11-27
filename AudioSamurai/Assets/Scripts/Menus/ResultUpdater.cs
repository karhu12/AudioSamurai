using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUpdater : MonoBehaviour
{
    public GameObject titleText;
    public GameObject scoreText;
    public GameObject comboText;
    public GameObject perfectsText;
    public GameObject normalsText;
    public GameObject poorsText;
    public GameObject missesText;
    public GameObject hitPercentageText;

    public void UpdateResult()
    {
        titleText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.MapName;
        scoreText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.Score.ToString();
        comboText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.HighestCombo.ToString();
        perfectsText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.perfects.ToString();
        normalsText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.normals.ToString();
        poorsText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.poors.ToString();
        missesText.GetComponent<Text>().text = ScoreSystem.Instance.gameResult.misses.ToString();
        hitPercentageText.GetComponent<Text>().text = $"{ScoreSystem.Instance.gameResult.RoundedHitPercentage.ToString()} %";
    }
}
