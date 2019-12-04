using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using EncryptStringSample;
using UnityEngine;

public class Mongo : Singleton<Mongo>
{
    private const string MONGO_URI = "mongodb+srv://admin:audiosamurai123@cluster0-cofmk.mongodb.net/test?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "highscoredb";

    private MongoClient client;
    private IMongoDatabase db;
    private IMongoCollection<PlayerRef> playerCollection;
    PlayerRef player;
    HighScoresCollection hsCollection;
    List<PlayerRef> playerList;
    public bool LoginSuccess { get; private set; }
    public bool SignInSuccess { get; private set; }

    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        playerCollection = db.GetCollection<PlayerRef>("highscores");
        ResetValues();
    }

    public void InsertUpdates(string playerName, HighScore highScore)
    {
        UpdatePlayerData(player, highScore);
    }

    public void UpdatePlayerData(PlayerRef player, HighScore highScore)
    {
        var playerFilter = Builders<PlayerRef>.Filter.Eq(x => x.Name, player.Name);
        var update = Builders<PlayerRef>.Update.Set(o => o.ScoreCollection, UpdateCollectionItem(hsCollection, highScore));
        var result = playerCollection.UpdateOneAsync(playerFilter, update).Result;
    }

    public HighScoresCollection UpdateCollectionItem(HighScoresCollection highScoresCollection, HighScore highScore)
    {
        var obj = highScoresCollection.Hiscores.FirstOrDefault(x => x.MapId == highScore.MapId);
        if(obj != null)
        {
            obj.Score = highScore.Score;
            obj.HighestCombo = highScore.HighestCombo;
            obj.MaxCombo = highScore.MaxCombo;
            obj.Perfects = highScore.Perfects;
            obj.Normals = highScore.Normals;
            obj.Poors = highScore.Poors;
            obj.Misses = highScore.Misses;
        }
        else
        {
            highScoresCollection.Hiscores.Add(new HighScore(highScore.MapId, highScore.Score, highScore.HighestCombo,highScore.MaxCombo, highScore.Perfects, highScore.Normals, highScore.Poors, highScore.Misses));
        }
        return highScoresCollection;
    }

    public PlayerRef GetPlayerByCredentials(string playerName, string pw)
    {
        playerList = playerCollection.Find(new BsonDocument()).ToList();
        foreach (var dox in playerList)
        {
            if (dox.Name.Equals(playerName))
            {
                var decrypted = PasswordHandler.Decrypt(dox.Password, dox.Name);
                if (decrypted.Equals(pw))
                {
                    LoginSuccess = true;
                    player = dox;
                    hsCollection = dox.ScoreCollection;
                }
                else
                {
                    LoginSuccess = false;
                }
            }
        }
        return player;
    }


    public void SetDataIfLogin(string playerName)
    {
        foreach (var dox in playerList)
        {
            if (dox.Name.Equals(playerName))
            {
                player = dox;
            }
        }
    }

    public bool CheckIfAvailable(string playerName)
    {
        foreach (var dox in playerList)
        {
            if (dox.Name.Equals(playerName))
            {
                SignInSuccess = false;
                break;
            }
            else
            {
                SignInSuccess = true;
            }
        }
        return SignInSuccess;
    }

    public HighScore GetPlayersMapScore(string mapName)
    {
        var playerObj = player;
        var scoreObj = playerObj.ScoreCollection.Hiscores.FirstOrDefault(x => x.MapId == mapName);
        if (scoreObj == null)
        {
            scoreObj = new HighScore(mapName, 0, 0, 0, 0, 0, 0, 0);
        }
        return scoreObj;
    }

    public async void RegisterNewPlayer(string playerName, string pw)
    {
        player.Name = playerName;
        player.Password = PasswordHandler.Encrypt(pw, player.Name);
        await playerCollection.InsertOneAsync(player);
        hsCollection = player.ScoreCollection;
    }

    public string GetPlayerName()
    {
        return player.Name;
    }

    public void ResetValues()
    {
        player = new PlayerRef();
        LoginSuccess = false;
        SignInSuccess = false;
        playerList = new List<PlayerRef>();
    }
}