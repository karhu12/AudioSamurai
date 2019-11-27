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
    public String FormattedHighscore { get; private set; }

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
        HighScore hs = mongo.GetPlayersMapScore("nahkapeitturi22", mapName);
        SetCurrentHighs(hs);
        if (newScore >= CurrentHighScore)
        {
            SetNewHighScore(mapName, newScore, newHighScores);
        }
    }

    private void SetNewHighScore(String mapName, int newScore ,List<String> list)
    {
        try
        {
            mongo.Insert("nahkapeitturi22", new HighScore(mapName, GameData.Instance.FinalScore, GameData.Instance.RoundedHitPercentage, GameData.Instance.HighestCombo));
        }
        catch (Exception) { }
    }

    /*private String GetScoreString(String mapName)
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
    }*/

    public void SetCurrentHighs (HighScore hs) //String mapName
    {
        try
        {
            CurrentHighScore = hs.Score;
            CurrentCombo = hs.Combo;
            CurrentHitPercentage = hs.HitP;
            Format();
        } catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Format()
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        FormattedHighscore = CurrentHighScore.ToString("#,0", nfi);
    }
}
