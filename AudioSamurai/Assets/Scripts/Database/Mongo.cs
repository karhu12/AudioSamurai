using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using UnityEngine;

public class Mongo
{
    private const string MONGO_URI = "mongodb+srv://tmett:Paskatukka666@cluster0-cofmk.mongodb.net/test?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "highscoredb";

    private MongoClient client;
    private IMongoDatabase db;
    private IMongoCollection<Model_Highscore> scoreCollection;
    Model_Highscore model = new Model_Highscore();

    private int score;

    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        scoreCollection = db.GetCollection<Model_Highscore>("highscores");
        Debug.Log("Database has been initialized");
    }

    public void Shutdown()
    {
    }

    public void InsertHighScore(string mapName, int highscore)
    {
        model.MapId = mapName;
        model.HighScore = highscore;
        scoreCollection.InsertOne(model);
    }

    public void Update(string mapName, int highscore)
    {
         model.MapId = mapName;
         model.HighScore = highscore;
         var filter = Builders<Model_Highscore>.Filter.Eq(x => x.MapId, mapName);
         var update = Builders<Model_Highscore>.Update.Set(o => o.HighScore, highscore);
         var result = scoreCollection.UpdateOneAsync(filter, update).Result;
    }

    public int GetCurrentHighScore(string mapName)
    {
        var list = scoreCollection.Find(new BsonDocument()).ToList();
        foreach (var dox in list)
        {
            if (dox.MapId.Equals(mapName))
            {
                score = dox.HighScore;
            }
        }
        return score;
    }
}