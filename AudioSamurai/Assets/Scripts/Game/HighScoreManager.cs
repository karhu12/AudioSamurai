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

    private Mongo mongo;

    private void Start()
    {
        mongo = new Mongo();
        mongo.Init();
    }

    //Compare new score from the previous run to the current highscore available for that map.
    public void CompareToHighScore(String mapName, int newScore)
    {
        SetCurrentHighs("isohannu", mapName);
        if (newScore >= CurrentHighScore)
        {
            SetNewHighScore(mapName);
        }
    }

    //Set new highscore data if the previous highscore on a map was beaten by the player.
    private void SetNewHighScore(String mapName)
    {
        try
        {
            mongo.Insert("isohannu", new HighScore(mapName, GameData.Instance.FinalScore, GameData.Instance.RoundedHitPercentage, GameData.Instance.HighestCombo));
        }
        catch (Exception) { }
    }

    //Get player's current highscore data on a certain map from mongodb and set to those values to current highs. 
    public void SetCurrentHighs (String playerName, String mapName)
    {
        try
        {
            HighScore hs = mongo.GetPlayersMapScore(playerName, mapName);
            CurrentHighScore = hs.Score;
            CurrentCombo = hs.Combo;
            CurrentHitPercentage = hs.HitP;
            Format();
        } catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    //Change the score format so that numbers are grouped in threes. Makes it clearer to read when displayed on the screen.
    private void Format()
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        FormattedHighscore = CurrentHighScore.ToString("#,0", nfi);
    }
}
