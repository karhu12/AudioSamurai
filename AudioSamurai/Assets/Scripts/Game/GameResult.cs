using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult
{
    /* Constants */
    public const float PERFECT_PERC_LINE = 95;
    public const float AMAZING_PERC_LINE = 90;
    public const float GREAT_PERC_LINE = 80;
    public const float OKAY_PERC_LINE = 70;

    public const int EMPTY_VALUE = -1;

    /* Serialization */
    public const int SCORE_IDX = 0;
    public const int MAX_COMBO_IDX = 1;
    public const int COMBO_IDX = 2;
    public const int PERFECT_IDX = 3;
    public const int NORMAL_IDX = 4;
    public const int POOR_IDX = 5;
    public const int MISS_IDX = 6;

    public const int SERIALIZABLE_ITEMS = 7;


    public int Score { get; set; } = 0;
    public int MaxCombo { get; set; } = 0;
    private int highestCombo = 0;
    public int HighestCombo
    {
        get => highestCombo;
        set
        {
            if (value > highestCombo && value > 0)
            {
                highestCombo = value;
            }
        }
    }
    private float hitPercentage = 0;

    public ScoreSystem.ResultGrade ResultGrade { get; private set; } = ScoreSystem.ResultGrade.None;
    public string MapName { get; set; }
    public double RoundedHitPercentage { get; private set; } = 0;

    public int perfects = 0;
    public int normals = 0;
    public int poors = 0;
    public int misses = 0;

    public void CalculateResult()
    {
        hitPercentage = (perfects * 1f + normals * .66f + poors * 0.33f) / MaxCombo * 100;
        RoundedHitPercentage = Math.Round((double)hitPercentage, 1);
        if (hitPercentage >= PERFECT_PERC_LINE) {
            ResultGrade = ScoreSystem.ResultGrade.Perfect;
        } else if (hitPercentage > AMAZING_PERC_LINE && hitPercentage < PERFECT_PERC_LINE) {
            ResultGrade = ScoreSystem.ResultGrade.Amazing;
        } else if (hitPercentage > GREAT_PERC_LINE && hitPercentage <= AMAZING_PERC_LINE) {
            ResultGrade = ScoreSystem.ResultGrade.Great;
        } else if (hitPercentage > OKAY_PERC_LINE && hitPercentage <= GREAT_PERC_LINE) {
            ResultGrade = ScoreSystem.ResultGrade.Okay;
        } else {
            ResultGrade = ScoreSystem.ResultGrade.Poor;
        }
    }
    
    public void CountHit(int hitScore)
    {
        switch ((ScoreSystem.HitType)hitScore) {
            case ScoreSystem.HitType.Perfect:
                perfects++;
                break;
            case ScoreSystem.HitType.Normal:
                normals++;
                break;
            case ScoreSystem.HitType.Poor:
                poors++;
                break;
            default:
                misses++;
                break;
        }
    }

    public void Reset() {
        ResultGrade = ScoreSystem.ResultGrade.None;
        MaxCombo = 0;
        highestCombo = 0;
        perfects = 0;
        normals = 0;
        poors = 0;
        misses = 0;
    }

    public string Serialize(char separator = ';') 
    {
        string serialized = "";
        for (int i = 0; i < SERIALIZABLE_ITEMS; i++) 
        {
            switch (i) {
                case SCORE_IDX:
                    serialized += Score;
                    break;
                case MAX_COMBO_IDX:
                    serialized += MaxCombo;
                    break;
                case COMBO_IDX:
                    serialized += HighestCombo;
                    break;
                case PERFECT_IDX:
                    serialized += perfects;
                    break;
                case NORMAL_IDX:
                    serialized += normals;
                    break;
                case POOR_IDX:
                    serialized += poors;
                    break;
                case MISS_IDX:
                    serialized += misses;
                    break;
            }
            if (i + 1 < SERIALIZABLE_ITEMS)
                serialized += separator;
        }
        return serialized;
    }

    public static string GetEmptyResultSerialization(char separator = ';') 
    {
        string serialized = "";
        for (int i = 0; i < SERIALIZABLE_ITEMS; i++)
        {
            serialized += $"{EMPTY_VALUE}{(i + 1 != SERIALIZABLE_ITEMS ? "" + separator : "")}";
        }
        return serialized;
    }

    public static GameResult Deserialize(string gameResultStr, char separator = ';')
    {
        GameResult deserialized = new GameResult();
        var splitStrings = gameResultStr.Split(new [] { separator }, StringSplitOptions.None);
        try 
        {
            for (int i = 0; i < splitStrings.Length; i++) {
                switch (i) {
                    case SCORE_IDX:
                        deserialized.Score = int.Parse(splitStrings[i]);
                        break;
                    case MAX_COMBO_IDX:
                        deserialized.MaxCombo = int.Parse(splitStrings[i]);
                        break;
                    case COMBO_IDX:
                        deserialized.HighestCombo = int.Parse(splitStrings[i]);
                        break;
                    case PERFECT_IDX:
                        deserialized.perfects = int.Parse(splitStrings[i]);
                        break;
                    case NORMAL_IDX:
                        deserialized.normals = int.Parse(splitStrings[i]);
                        break;
                    case POOR_IDX:
                        deserialized.poors = int.Parse(splitStrings[i]);
                        break;
                    case MISS_IDX:
                        deserialized.misses = int.Parse(splitStrings[i]);
                        break;
                }
            }
        } catch (Exception e)
        {
            MonoBehaviour.print($"Exception while deserializing gameResult : {e.StackTrace}");
        }
        deserialized.CalculateResult();
        return deserialized;
    }
}
