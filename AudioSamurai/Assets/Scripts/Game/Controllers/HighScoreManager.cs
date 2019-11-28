using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HighScoreManager : Singleton<HighScoreManager>
{
    public void CompareToHighScore(GameResult newResult, GameResult oldResult)
    {
        if (newResult.Score >= oldResult.Score)
        {
            SetNewGameResult(newResult);
        }
    }

    public GameResult GetGameResult(String mapName)
    {
        string gameResult = PlayerPrefs.GetString(mapName);

        if (string.IsNullOrEmpty(gameResult))
        {
            PlayerPrefs.SetString(mapName, GameResult.GetEmptyResultSerialization());
            PlayerPrefs.Save();
            gameResult = PlayerPrefs.GetString(mapName);
        }

        return GameResult.Deserialize(gameResult);
    }

    public static string GetFormattedHighscore(GameResult result)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return result.Score.ToString("#,0", nfi);
    }
    private void SetNewGameResult(GameResult result) {
        PlayerPrefs.SetString(result.MapName, result.Serialize());
        PlayerPrefs.Save();
    }
}
