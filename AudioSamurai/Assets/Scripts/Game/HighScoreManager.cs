using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HighScoreManager : Singleton<HighScoreManager>
{
    public int CurrentCombo { get; private set; }
    public int CurrentHighScore { get; private set; }
    public double CurrentHitPercentage { get; private set; }
    public String formattedHighscore { get; private set; }

    private readonly Int32 count = 3;
    private String[] currentHighScores = new string[3];
    private List<String> newHighScores = new List<String>();
    private readonly String[] separator = { ";" };
    private String storable;
    Mongo mongo;

    private void Start()
    {
        mongo = new Mongo();
        mongo.Init();
    }

    public void CompareToHighScore(int newScore, String mapName)
    {
        SetCurrentHighs(mapName);
        int score = mongo.GetCurrentHighScore(mapName);
        if (newScore >= CurrentHighScore)
        {
            newHighScores.Clear();
            newHighScores.Add(newScore.ToString());
            newHighScores.Add(GameData.Instance.HighestCombo.ToString());
            newHighScores.Add(GameData.Instance.RoundedHitPercentage.ToString());
            SetNewHighScore(mapName, newScore, newHighScores);
        }
    }

    private void SetNewHighScore(String mapName, int newScore ,List<String> list)
    {
        storable = String.Join(";", list);
        try
        {
            mongo.InsertHighScore(mapName, newScore);
        } catch(Exception) { 
            mongo.Update(mapName, newScore); 
        }
        PlayerPrefs.SetString(mapName, storable);
        PlayerPrefs.Save();
    }

    private String GetScoreString(String mapName)
    {
        storable = PlayerPrefs.GetString(mapName);
        Debug.Log(storable);
        if (string.IsNullOrEmpty(storable))
        {
            PlayerPrefs.SetString(mapName, "0;0;0,0");
            PlayerPrefs.Save();
            storable = PlayerPrefs.GetString(mapName);
        }
        return storable;
    }

    public void SetCurrentHighs(String mapName)
    {
        currentHighScores = GetScoreString(mapName).Split(separator, count, StringSplitOptions.RemoveEmptyEntries);
        try
        {
            String score = Convert.ToString(currentHighScores[0]);
            CurrentHighScore = Convert.ToInt32(score);
            String combo = Convert.ToString(currentHighScores[1]);
            CurrentCombo = Convert.ToInt32(combo);
            String hitP = Convert.ToString(currentHighScores[2]);
            CurrentHitPercentage = Convert.ToDouble(hitP);
            Format();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Format()
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        formattedHighscore = CurrentHighScore.ToString("#,0", nfi);
    }
}
