using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUpdater : MonoBehaviour
{

    public GameObject scoreText;
    public GameObject comboText;
    public GameObject hitPercentageText;

    public void Update()
    {
        scoreText.GetComponent<Text>().text = "Score: " + GameData.Instance.FinalScore.ToString();
        comboText.GetComponent<Text>().text = "Highest combo: " + GameData.Instance.HighestCombo.ToString();
        hitPercentageText.GetComponent<Text>().text = "Hit percentage: " + GameData.Instance.RoundedHitPercentage.ToString();
    }
}
