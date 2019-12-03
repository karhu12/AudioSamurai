using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HighScoreManager : Singleton<HighScoreManager>
{
    //Mongo mongo = new Mongo();
    /* Compares old gameResult to a new one. Returns true and saves the result if its the new highscore and false if not. */
    public bool CompareToHighScore(GameResult newResult, GameResult oldResult)
    {
        if (newResult.Score > oldResult.Score)
        {
            SetNewGameResult(newResult);
            return true;
        }
        return false;
    }

    /* Returns gameResult of given map if it exists. Otherwise returns default empty result. */
    public GameResult GetGameResult(String mapName)
    {
        HighScore hs;
        GameResult result = new GameResult();
        hs = Mongo.Instance.GetPlayersMapScore(mapName);
        result.MapName = hs.MapId;
        result.HighestCombo = hs.HighestCombo;
        result.MaxCombo = hs.MaxCombo;
        result.Score = hs.Score;
        result.perfects = hs.Perfects;
        result.normals = hs.Normals;
        result.poors = hs.Poors;
        result.misses = hs.Misses;
        if (result.Score > 0)
        {
            result.CalculateResult();
        }
        //result.MapName = mapName;
        
        return result;
        /*string gameResult = PlayerPrefs.GetString(mapName);

        if (string.IsNullOrEmpty(gameResult))
        {
            PlayerPrefs.SetString(mapName, GameResult.GetEmptyResultSerialization());
            PlayerPrefs.Save();
            gameResult = PlayerPrefs.GetString(mapName);
        }*/

        //GameResult.Deserialize(gameResult);
    }

    /* Returns an formatted highscore result from given result. */
    public static string GetFormattedHighscore(GameResult result)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return result.Score.ToString("#,0", nfi);
    }

    /* Saves given result as new highscore. */
    private void SetNewGameResult(GameResult result) {
        Mongo.Instance.InsertUpdates(Mongo.Instance.ReturnPlayerName(), new HighScore(result.MapName, result.Score, result.HighestCombo, result.MaxCombo, result.perfects, result.normals, result.poors, result.misses));
        //PlayerPrefs.SetString(result.MapName, result.Serialize());
        //PlayerPrefs.Save();
    }
}
