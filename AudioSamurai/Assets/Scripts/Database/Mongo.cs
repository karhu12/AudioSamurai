using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using UnityEngine;

public class Mongo
{
    private const string MONGO_URI = "mongodb+srv://tmett:Paskatukka666@cluster0-cofmk.mongodb.net/test?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "highscoredb";

    private MongoClient client;
    //private MongoServer server;
    private IMongoDatabase db;
    private IMongoCollection<Model_Highscore> scoreCollection;
    Model_Highscore model = new Model_Highscore();

    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        scoreCollection = db.GetCollection<Model_Highscore>("highscores");
        Debug.Log("Database has been initialized");
        InsertHighScore("Faded", 1000000);
    }

    public void Shutdown()
    {

    }

    #region
    public void InsertHighScore(string mapName, int highscore)
    {
        model.MapName = mapName;
        model.HighScore = highscore;
        scoreCollection.InsertOne(model);
    }
    #endregion


    #region
    public void Update(string mapName, int highscore)
    {
        model.MapName = mapName;
        model.HighScore = highscore;
        var filter = Builders<Model_Highscore>.Filter.Eq(s => s.MapName, mapName);
        scoreCollection.UpdateOne(filter, model);
    }
    #endregion
}