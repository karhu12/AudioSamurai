using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject multiplierText;
    public GameObject comboText;
    
    //Minimum and maximum values for combo.
    public const int MIN_MULTIPLIER = 1;
    public const int MAX_MULTIPLIER = 16;

    public static int multiplier;
    public static int combo;
    public static int score;
    //Call this from other class when you want the game to start.
    public bool hasGameStarted = false;
    public bool isLevelFinished = false;
    public int testmultiplier = 1; //Just for testing purposes.

    private void Start()
    {
        multiplier = MIN_MULTIPLIER;
        combo = 0;
        score = 0;
        //Add score every 0.1 seconds. 
        InvokeRepeating("AddScore", 1, 0.1f);  
    }

    void Update()
    {
        scoreText.GetComponent<Text>().text = "SCORE: " + score;
        multiplierText.GetComponent<Text>().text = "MULTIPLIER: " + multiplier + "x";
        comboText.GetComponent<Text>().text = "COMBO: " + combo;
    }

    //Add basic score from proggressing. Combo, hits and misses are handled in MapObject script.
    void AddScore()
    {
        if (hasGameStarted && !isLevelFinished)
        {
            scoreText.SetActive(true);
            comboText.SetActive(true);
            multiplierText.SetActive(true);
            multiplier = testmultiplier; //Just for testing purposes.
            score += 1 * multiplier;
        }
        else if(isLevelFinished)
        {
            Debug.Log("Your score was: " + score);
        }
    }
}
