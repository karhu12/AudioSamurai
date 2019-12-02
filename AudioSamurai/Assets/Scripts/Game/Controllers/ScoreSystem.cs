using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : Singleton<ScoreSystem>
{
    /* Constants */
    public const int HIT_TYPES = 3;
    public const int NO_FAIL_MODE_PENALTY_DIVIDER = 2;
    public const string PERFECT_TEXT = "Perfect";
    public const string NORMAL_TEXT = "Normal";
    public const string POOR_TEXT = "Poor";
    public const string MISS_TEXT = "X";
    

    public Texture perfectTexture;
    public Texture amazingTexture;
    public Texture greatTexture;
    public Texture okayTexture;
    public Texture poorTexture;

    public enum HitType
    {
        Perfect = 300,
        Normal = 100,
        Poor = 50,
        Miss = 0
    }

    public enum ResultGrade
    {
        Perfect,
        Amazing,
        Great,
        Okay,
        Poor,
        None
    }

    /* Returns the image texture matching to the given grade */
    public Texture GetResultGradeTexture(ResultGrade grade)
    {
        switch (grade) {
            case ResultGrade.Perfect:
                if (perfectTexture != null)
                    return perfectTexture;
                break;
            case ResultGrade.Amazing:
                if (amazingTexture != null)
                    return amazingTexture;
                break;
            case ResultGrade.Great:
                if (greatTexture != null)
                    return greatTexture;
                break;
            case ResultGrade.Okay:
                if (okayTexture != null)
                    return okayTexture;
                break;
            case ResultGrade.Poor:
                if (poorTexture != null)
                    return poorTexture;
                break;
        }
        return null;
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
    public GameResult gameResult { get; private set; }

    private int combo;
    private int score;

    private new void Awake() {
        base.Awake();
        gameResult = new GameResult();
    }

    private void Start()
    {
        comboAnim = comboText.GetComponent<Animator>();
        comboAnim.Play("New state");
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
            if (ModeManager.Instance.GetMode() == 2)
                scoreToAdd = scoreToAdd / NO_FAIL_MODE_PENALTY_DIVIDER;
            score += scoreToAdd * (combo + 1);
            combo += 1;
            comboAnim.Play("comboAnimation");
            gameResult.HighestCombo = combo;
            gameResult.CountHit(scoreToAdd);
        }
    }

    /* Saves the gameResult score, calculates accuracy and compares it to old highscore. Returns boolean if its the new highscore. */
    public bool FinalizeResult()
    {
        gameResult.MapName = GameController.Instance.SelectedSongmap.GetSongmapName();
        gameResult.Score = score;
        gameResult.CalculateResult();
        return HighScoreManager.Instance.CompareToHighScore(gameResult, HighScoreManager.Instance.GetGameResult(gameResult.MapName));
    }

    public void AddScore(HitType hit)
    {
        AddScore((int)hit);
    }

    public int GetScore()
    {
        return score;
    }

    public void Miss() {
        combo = 0;
        gameResult.CountHit((int)HitType.Miss);
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    public void ResetScore() {
        score = 0;
        gameResult.Reset();
    }
}
