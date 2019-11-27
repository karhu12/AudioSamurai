using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult
{
    public int Score { get; set; } = 0;
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

    public string MapName { get; set; }
    public double RoundedHitPercentage { get; private set; } = 0;

    public int perfects = 0;
    public int normals = 0;
    public int poors = 0;
    public int misses = 0;

    public void CalculateHitPercentage(int maxCombo)
    {
        hitPercentage = (perfects * 1f + normals * .66f + poors * 0.33f) / maxCombo * 100;
        RoundedHitPercentage = Math.Round((double)hitPercentage, 1);
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
        highestCombo = 0;
        perfects = 0;
        normals = 0;
        poors = 0;
        misses = 0;
    }
}
