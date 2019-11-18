using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public GameObject scoreText;
    public TextMeshProUGUI comboText;
    public static Animator comboAnim;

    private static int combo;
    private static int score;
    //Call this from other class where the game starts so that the counting starts at the same time.
    public static bool isGameOnGoing = false;

    private void Start()
    {
        comboAnim = comboText.GetComponent<Animator>();
        combo = 0;
        score = 0;
    }

    void Update()
    {
        AddScore(1);
        scoreText.GetComponent<Text>().text = score.ToString();
        comboText.GetComponent<TextMeshProUGUI>().text = combo.ToString() + "x";
    }

    public static void AddScore(int scoreToAdd)
    {
        if (isGameOnGoing)
        {
            if (combo > 0)
            {
                score += scoreToAdd * combo;
            }
            else
            {
                score += scoreToAdd;
            }
            
        }
    }

    public static void AddCombo()
    {
        combo += 1;
        comboAnim.Play("comboAnimation");
    }

    public static void ResetCombo()
    {
        combo = 0;
    }
}
