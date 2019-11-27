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
    public GameObject gradeImage;
    public GameObject gradeText;

    public void UpdateResult()
    {
        GameResult result = ScoreSystem.Instance.gameResult;
        titleText.GetComponent<Text>().text = result.MapName;
        scoreText.GetComponent<Text>().text = result.Score.ToString();
        comboText.GetComponent<Text>().text = result.HighestCombo.ToString();
        perfectsText.GetComponent<Text>().text = result.perfects.ToString();
        normalsText.GetComponent<Text>().text = result.normals.ToString();
        poorsText.GetComponent<Text>().text = result.poors.ToString();
        missesText.GetComponent<Text>().text = result.misses.ToString();
        hitPercentageText.GetComponent<Text>().text = $"{result.RoundedHitPercentage.ToString()} %";
        gradeImage.GetComponent<RawImage>().texture = ScoreSystem.Instance.GetResultGradeTexture(result.ResultGrade);
        gradeText.GetComponent<Text>().text = result.ResultGrade.ToString();
    }
}
