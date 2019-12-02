using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUpdater : MonoBehaviour
{
    public Text titleText;
    public Text scoreText;
    public Text comboText;
    public Text perfectsText;
    public Text normalsText;
    public Text poorsText;
    public Text missesText;
    public Text hitPercentageText;
    public RawImage gradeImage;
    public Text gradeText;
    public Text newHighscore;
    public Image highscoreHighlight;

    public void UpdateResult(bool isHighScore = false)
    {
        GameResult result = ScoreSystem.Instance.gameResult;
        titleText.text = result.MapName;
        scoreText.text = result.Score.ToString();
        comboText.text = result.HighestCombo.ToString();
        perfectsText.text = result.perfects.ToString();
        normalsText.text = result.normals.ToString();
        poorsText.text = result.poors.ToString();
        missesText.text = result.misses.ToString();
        hitPercentageText.text = $"{result.RoundedHitPercentage.ToString()} %";
        gradeImage.texture = ScoreSystem.Instance.GetResultGradeTexture(result.ResultGrade);
        gradeText.text = result.ResultGrade.ToString();

        if (isHighScore) 
        {
            newHighscore.gameObject.SetActive(true);
            highscoreHighlight.CrossFadeAlpha(255, 0, true);
            FindObjectOfType<AudioManager>().Play("NewHighscore");
        } else {
            newHighscore.gameObject.SetActive(false);
            highscoreHighlight.CrossFadeAlpha(0, 0, true);
        }
    }
}
