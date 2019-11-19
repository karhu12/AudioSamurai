using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    private string mapName = "";
    private float finalScore = 0;
    private float hitPercentage = 0;
    private double roundedHitPercentage = 0;
    private float highestCombo = 0;
    private float hits = 0;
    private float misses = 0;

    public void OnSuccessfulHit()
    {
        hits++;
        Debug.Log(hits);
    }

    public void OnHitMissed()
    {
        misses++;
        Debug.Log(misses);
    }

    public void CalculateHitPercentage()
    {
        hitPercentage = hits / (hits + misses) * 100;
        roundedHitPercentage = Math.Round((double)hitPercentage, 1);
    }

    public void CompareToHighestCombo(float combo)
    {
        if (combo > highestCombo)
        {
            highestCombo = combo;
        }
    }

    public void ResetHitsAndMisses()
    {
        hits = 0;
        misses = 0;
    }


    public double GetRoundedHitPercentage()
    {
        return roundedHitPercentage;
    }

    public float GetHighestCombo()
    {
        return highestCombo;
    }

    public float GetFinalScore()
    {
        return finalScore;
    }

    public void SetFinalScore(float score)
    {
        if(score != 0) { finalScore = score; }
    }

    public string GetMapName()
    {
        return mapName;
    }

    public void SetMapName(string nameOfMap)
    {
        mapName = nameOfMap;
    }
}
