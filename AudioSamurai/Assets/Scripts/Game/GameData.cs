using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    public float FinalScore { get; set; } = 0;
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
    public string MapName { get; set; } = "";
    public double RoundedHitPercentage { get; private set; } = 0;
    
    private float hitPercentage = 0;
    private float hits = 0;
    private float misses = 0;

    public void OnSuccessfulHit()
    {
        hits++;
    }

    public void OnHitMissed()
    {
        misses++;
    }

    public void CalculateHitPercentage()
    {
        hitPercentage = hits / (hits + misses) * 100;
        RoundedHitPercentage = Math.Round((double)hitPercentage, 1);
    }

    public void ResetHitsAndMisses()
    {
        hits = 0;
        misses = 0;
    }
}
