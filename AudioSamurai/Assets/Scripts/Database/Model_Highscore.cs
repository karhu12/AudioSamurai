using MongoDB.Bson;

public class Model_Highscore
{
    public ObjectId _id;

    public string MapName { get; set; }
    public int HighScore { get; set; }
}
