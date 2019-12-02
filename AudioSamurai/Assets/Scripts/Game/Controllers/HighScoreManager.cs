using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HighScoreManager : Singleton<HighScoreManager>
{
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
        string gameResult = PlayerPrefs.GetString(mapName);

        if (string.IsNullOrEmpty(gameResult))
        {
            PlayerPrefs.SetString(mapName, GameResult.GetEmptyResultSerialization());
            PlayerPrefs.Save();
            gameResult = PlayerPrefs.GetString(mapName);
        }

        GameResult result = GameResult.Deserialize(gameResult);
        result.MapName = mapName;
        return result;
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
        PlayerPrefs.SetString(result.MapName, result.Serialize());
        PlayerPrefs.Save();
    }
}
