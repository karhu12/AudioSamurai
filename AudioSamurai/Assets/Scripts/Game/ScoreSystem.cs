using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject multiplierText;
    public GameObject comboText;
    public GameObject hudCanvas;
    
    //Minimum and maximum values for combo.
    public const int MIN_MULTIPLIER = 1;
    public const int MAX_MULTIPLIER = 16;

    public static int multiplier;
    public static int combo;
    public static int score;
    //Call this from other class where the game starts so that the counting starts at the same time.
    public bool hasGameStarted = false;
    public bool isLevelFinished = false;
    public int testmultiplier = 1; //Just for testing purposes.


    public GameObject health;
    private HealthBarController hbc;

    private void Start()
    {
        hbc = health.GetComponent<HealthBarController>();
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
            if (!hbc.isGameOver)
            {
                hudCanvas.SetActive(true);
                multiplier = testmultiplier; //Just for testing purposes.
                score += 1 * multiplier;
            }
            else
            {
                CancelInvoke();
                Debug.Log("GAME OVER");
            }
        }
        else if(isLevelFinished)
        {
            Debug.Log("Your score was: " + score);
        }
    }
}
