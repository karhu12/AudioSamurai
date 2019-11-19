using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : Singleton<ScoreSystem>
{
    public enum HitType
    {
        Perfect = 300,
        Normal = 100,
        Poor = 50
    }

    public GameObject scoreText;
    public TextMeshProUGUI comboText;
    public Animator comboAnim;

    private int combo;
    private int score;

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

    public void AddScore(int scoreToAdd)
    {
        if (GameController.Instance.State == GameController.GameState.Playing)
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

    public void AddScore(HitType hit)
    {
        AddScore((int)hit);
    }

    public void AddCombo()
    {
        if (GameController.Instance.State == GameController.GameState.Playing)
        {
            combo += 1;
            comboAnim.Play("comboAnimation");
            GameData.Instance.CompareToHighestCombo(combo);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    public void ResetScore() {
        score = 0;
    }
}
