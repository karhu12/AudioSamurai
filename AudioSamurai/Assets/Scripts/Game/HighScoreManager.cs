using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : Singleton<HighScoreManager>
{
    private String[] currentHighScores = new string[3];
    private List<String> newHighScores = new List<String>();
    private String storable;
    private String[] separator = { ";" };
    private Int32 count = 3;
    public int currentHighScore { get; private set; }
    public int currentCombo { get; private set; }
    public double currentHitPercentage { get; private set; }

    public void CompareToHighScore(int newScore, String mapName)
    {
        Debug.Log(GetScores(mapName));
        currentHighScores = GetScores(mapName).Split(separator, count ,StringSplitOptions.RemoveEmptyEntries);
        try
        {
            String score = Convert.ToString(currentHighScores[0]);
            currentHighScore = Convert.ToInt32(score);
            String combo = Convert.ToString(currentHighScores[1]);
            currentCombo = Convert.ToInt32(combo);
            String hitP = Convert.ToString(currentHighScores[2]);
            currentHitPercentage = Convert.ToDouble(hitP);
        } catch(Exception e)
        {
            Debug.Log(e);
        }
        if (newScore >= currentHighScore)
        {
            newHighScores.Clear();
            newHighScores.Add(newScore.ToString());
            newHighScores.Add(GameData.Instance.HighestCombo.ToString());
            newHighScores.Add(GameData.Instance.RoundedHitPercentage.ToString());
            SetNewHighScore(mapName, newHighScores);
        }
    }

    private void SetNewHighScore(String mapName, List<String> list)
    {
        storable = String.Join(";", list);
        PlayerPrefs.SetString(mapName, storable);
        PlayerPrefs.Save();
        Debug.Log(GetScores(mapName));
    }

    private String GetScores(String mapName)
    {
        String scoreString = PlayerPrefs.GetString(mapName);
        if (!string.IsNullOrEmpty(scoreString))
        {
            storable = PlayerPrefs.GetString(mapName);
        }
        else 
        {
            PlayerPrefs.SetString(mapName, "0;0;0,0");
            PlayerPrefs.Save();
            storable = PlayerPrefs.GetString(mapName);
        }
        return storable;
    }
}
