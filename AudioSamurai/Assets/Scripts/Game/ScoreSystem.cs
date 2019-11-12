using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject comboText;

    //Minimum and maximum values for combo.
    public const int MIN_COMBO = 1;
    public const int MAX_COMBO = 16;

    public static int combo;
    public static int score;
    //Call this from other class when you want the game to start.
    public bool hasGameStarted = false;
    public bool isLevelFinished = false;
    //public int testcombo; Just for testing purposes.

    private void Start()
    {
        combo = MIN_COMBO;
        score = 0;
        //Add score every 0.1 seconds. 
        InvokeRepeating("AddScore", 1, 0.1f);  
    }

    void Update()
    {
        scoreText.GetComponent<Text>().text = "SCORE: " + score;
        comboText.GetComponent<Text>().text = "COMBO: " + combo + "x"; 
    }

    //Add basic score from proggressing. Combo, hits and misses are handled in MapObject script.
    void AddScore()
    {
        if (hasGameStarted && !isLevelFinished)
        {
            //combo = testcombo; Just for testing purposes.
            scoreText.SetActive(true);
            comboText.SetActive(true);
            score += 1 * combo;
        }
        else if(isLevelFinished)
        {
            Debug.Log("Your score was: " + score);
        }
    }
}
