using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : Singleton<ScoreSystem>
{
    /* Constants */
    public const int HIT_TYPES = 3;
    public const string PERFECT_TEXT = "Perfect";
    public const string NORMAL_TEXT = "Normal";
    public const string POOR_TEXT = "Poor";
    public const string MISS_TEXT = "X";

    public enum HitType
    {
        Perfect = 300,
        Normal = 100,
        Poor = 50,
        Miss = 0
    }

    public static string GetHitTypeString(HitType hitType)
    {
        switch (hitType) {
            case HitType.Perfect:
                return PERFECT_TEXT;
            case HitType.Normal:
                return NORMAL_TEXT;
            case HitType.Poor:
                return POOR_TEXT;
        }
        return MISS_TEXT;
    }

    public static Color GetHitTypeColor(HitType hitType)
    {
        switch (hitType) {
            case HitType.Perfect:
                return Color.cyan;
            case HitType.Normal:
                return Color.green;
            case HitType.Poor:
                return Color.yellow;
        }
        return Color.red;
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
        if (scoreText == null)
        {
            
        }

        else
        {
            scoreText.GetComponent<Text>().text = score.ToString();
            comboText.GetComponent<TextMeshProUGUI>().text = combo.ToString() + "x";
        }
        
    }

    public void AddScore(int scoreToAdd)
    {
        if (GameController.Instance.State == GameController.GameState.Playing)
        {
            score += scoreToAdd * (combo == 0 ? 1 : combo);
            combo += 1;
            comboAnim.Play("comboAnimation");
            GameData.Instance.HighestCombo = combo;
        }
    }

    public void AddScore(HitType hit)
    {
        AddScore((int)hit);
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
