using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

    public double HitP { get; set; }

    public int Combo { get; set; }

    public HighScore(string map, int score, double hitp, int combo)
    {
        this.MapId = map;
        this.Score = score;
        this.HitP = hitp;
        this.Combo = combo;
    }
}