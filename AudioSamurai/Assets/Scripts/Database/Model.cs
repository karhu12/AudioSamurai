using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

public class PlayerRef
{
    [BsonId]
    public string Name { get; set; }

    public string Password { get; set; }
    public HighScoresCollection ScoreCollection { get; set; }

    public PlayerRef()
    {
        ScoreCollection = new HighScoresCollection();
    }
}

public class HighScoresCollection
{
    public List<HighScore> Hiscores { get; set; }

    public HighScoresCollection()
    {
         Hiscores = new List<HighScore>();
    }
}

[BsonIgnoreExtraElements]
public class HighScore
{
    [BsonId]
    public string MapId { get; set; }
    public int Score { get; set; }
    public int HighestCombo { get; set; }
    public int MaxCombo { get; set; }
    public int Perfects { get; set; }
    public int Normals { get; set; }
    public int Poors { get; set; }
    public int Misses { get; set; }

    public HighScore(string map, int score, int highestCombo, int maxCombo, int perfects, int normals, int poors, int misses)
    {
        this.MapId = map;
        this.Score = score;
        this.HighestCombo = highestCombo;
        this.MaxCombo = maxCombo;
        this.Perfects = perfects;
        this.Normals = normals;
        this.Poors = poors;
        this.Misses = misses;
    }
}

public class LeaderBoardItem
{
    public string Name { get; set; }
    public int Score { get; set; }

    public LeaderBoardItem(string name, int score)
    {
        this.Name = name;
        this.Score = score;
    }
}