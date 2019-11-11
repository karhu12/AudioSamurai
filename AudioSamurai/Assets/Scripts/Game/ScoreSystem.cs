using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject comboText;

    public const int MIN_COMBO = 1;
    public const int MAX_COMBO = 16;

    public static int combo;
    public static int score;

    private void Start()
    {
        combo = MIN_COMBO;
        score = 0;
    }

    void Update()
    {
        scoreText.GetComponent<Text>().text = "SCORE: " + score;
        comboText.GetComponent<Text>().text = "COMBO: " + combo + "x";
        AddScore();
    }

    //Add basic score from proggressing. Combo, hits and misses are handled in MapObject script.
    void AddScore()
    {
        score += 1 * combo;
    }
}
