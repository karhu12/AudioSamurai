using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class Model_Highscore
{
    /*First implementation with players personal hiscores, then if we have time left maybe try to implement global leaderboard for all players.
     * 
     **/
    [BsonId]
    public string MapId { get; set; }
    public int HighScore { get; set; }
}
