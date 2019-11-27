using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

public class PlayerRef
{
    [BsonId]
    public string Name { get; set; }
    public HighScoresCollection highScoresCollection { get; set; }

    public PlayerRef()
    {
        highScoresCollection = new HighScoresCollection();
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
    /*First implementation with players personal hiscores, then if we have time left maybe try to implement global leaderboard for all players.
     * 
     **/
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