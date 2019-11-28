using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mongo
{
    private const string MONGO_URI = "mongodb+srv://admin:audiosamurai123@cluster0-cofmk.mongodb.net/test?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "highscoredb";

    private MongoClient client;
    private IMongoDatabase db;
    private IMongoCollection<PlayerRef> playerCollection;
    PlayerRef player;
    HighScoresCollection hsCollection;

    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        playerCollection = db.GetCollection<PlayerRef>("highscores");
    }

    public void Insert(string playerName, HighScore highScore)
    {
        GetPlayerByName(playerName);
        UpdatePlayerData(player, highScore);
        playerCollection.InsertOne(player);
    }

    public void UpdatePlayerData(PlayerRef player, HighScore highScore)
    {
        var playerFilter = Builders<PlayerRef>.Filter.Eq(x => x.Name, player.Name);
        var update = Builders<PlayerRef>.Update.Set(o => o.highScoresCollection, UpdateCollectionItem(hsCollection, highScore));
        var result = playerCollection.UpdateOneAsync(playerFilter, update).Result;
    }

    public HighScoresCollection UpdateCollectionItem(HighScoresCollection highScoresCollection, HighScore highScore)
    {
        var obj = highScoresCollection.Hiscores.FirstOrDefault(x => x.MapId == highScore.MapId);
        if(obj != null)
        {
            obj.Score = highScore.Score;
            obj.Combo = highScore.Combo;
            obj.HitP = highScore.HitP;
        }
        else
        {
            highScoresCollection.Hiscores.Add(new HighScore(highScore.MapId, highScore.Score, highScore.HitP, highScore.Combo));
        }
        return highScoresCollection;
    }

    public PlayerRef GetPlayerByName(string playerName)
    {
        bool isMatch = false;
        var list = playerCollection.Find(new BsonDocument()).ToList();
        foreach (var dox in list)
        {
            if (dox.Name.Equals(playerName))
            {
                isMatch = true;
                player = dox;
                hsCollection = dox.highScoresCollection;
            }
        }
        if(isMatch == false)
        {
            player = new PlayerRef();
            player.Name = playerName;
            playerCollection.InsertOne(player);
            hsCollection = player.highScoresCollection;
        }
        return player;
    }

    public HighScore GetPlayersMapScore(string playerName, string mapName)
    {
        var playerObj = GetPlayerByName(playerName);
        var scoreObj = playerObj.highScoresCollection.Hiscores.FirstOrDefault(x => x.MapId == mapName);
        if (scoreObj == null)
        {
            scoreObj = new HighScore(mapName, 0, 0, 0);
        }
        return scoreObj;
    }
}