using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using EncryptStringSample;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class Mongo : Singleton<Mongo>
{
    public bool LoginSuccess { get; private set; }
    public bool SignInSuccess { get; private set; }
    public bool TaskFinished { get; private set; }
    private const string MONGO_URI = "mongodb+srv://admin:audiosamurai123@cluster0-cofmk.mongodb.net/test?retryWrites=true&w=majority";
    private const string DATABASE_NAME = "highscoredb";
    private MongoClient client;
    private IMongoDatabase db;
    private IMongoCollection<PlayerRef> playerCollection;
    private PlayerRef player;
    private HighScoresCollection hsCollection;
    private List<PlayerRef> playerList;
    private List<int> leaderBoards;
    private List<LeaderBoardItem> lb;

    //Establish connection to the database and get collection of playeritems. Initialize key variables in ResetValues method.
    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        playerCollection = db.GetCollection<PlayerRef>("highscores");
        ResetValues();
    }

    //Called when there is something to update to the cloud.
    public void InsertUpdates(HighScore highScore)
    {
        UpdatePlayerData(player, highScore);
    }

    //Filter the player currently logged in from the collection so that we can update the correct player's data. Insert updated version of the player's highscore collection.
    public void UpdatePlayerData(PlayerRef player, HighScore highScore)
    {
        var playerFilter = Builders<PlayerRef>.Filter.Eq(x => x.Name, player.Name);
        var update = Builders<PlayerRef>.Update.Set(o => o.ScoreCollection, UpdateCollectionItem(hsCollection, highScore));
        var result = playerCollection.UpdateOneAsync(playerFilter, update).Result;
    }

    //Search player's highscore on certain map and update it.
    public HighScoresCollection UpdateCollectionItem(HighScoresCollection highScoresCollection, HighScore highScore)
    {
        var obj = highScoresCollection.Hiscores.FirstOrDefault(x => x.MapId == highScore.MapId);
        if (obj != null)
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
            highScoresCollection.Hiscores.Add(new HighScore(highScore.MapId, highScore.Score, highScore.HighestCombo, highScore.MaxCombo, highScore.Perfects, highScore.Normals, highScore.Poors, highScore.Misses));
        }
        return highScoresCollection;
    }

    //Verify if user put the correct credentials on login menu and return a playerobject matching those credentials from the cloud if he did.
    public PlayerRef GetPlayerByCredentials(string playerName, string pw)
    {
        TaskFinished = false;
        playerList = playerCollection.Find(new BsonDocument()).ToList();
        foreach (var playerObject in playerList)
        {
            if (playerObject.Name.Equals(playerName))
            {
                var decrypted = PasswordHandler.Decrypt(playerObject.Password, playerObject.Name);
                if (decrypted.Equals(pw))
                {
                    LoginSuccess = true;
                    player = playerObject;
                    hsCollection = playerObject.ScoreCollection;
                }
                else
                {
                    LoginSuccess = false;
                }
            }
        }
        TaskFinished = true;
        return player;
    }

    //Used in a situation where the player has already logged in when the game starts. Login controller saves the login state to player prefs.
    public void SetDataIfLogin(string playerName)
    {
        LoginSuccess = true;
        playerList = playerCollection.Find(new BsonDocument()).ToList();
        foreach (var playerObject in playerList)
        {
            if (playerObject.Name.Equals(playerName))
            {
                player = playerObject;
                hsCollection = player.ScoreCollection;
            }
        }
    }

    //Check if a username is available when trying to sign up (Check that there isn't duplicate in the database already)
    public async Task<bool> CheckIfAvailable(string playerName)
    {
        playerList = await playerCollection.Find(new BsonDocument()).ToListAsync();
        if (playerList.Count > 0)
        {
            foreach (var playerObject in playerList)
            {
                if (playerObject.Name.Equals(playerName))
                {
                    SignInSuccess = false;
                    break;
                }
                else
                {
                    SignInSuccess = true;
                }
            }
        }
        else
        {
            SignInSuccess = true;
        }
        return SignInSuccess;
    }

    //Get player's current highscore in a map. If there is no match for highscore, default zero-valued highscore is created.
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

    //Get the top 5 scores and scoreholders of a map between all players and sort the list.
    public async Task<List<LeaderBoardItem>> GetLeaderBoards(string mapName)
    {
        SetupLeaderboards();
        playerList = await playerCollection.Find(new BsonDocument()).ToListAsync();
        foreach (var playerObject in playerList)
        {
            var scoreObj = playerObject.ScoreCollection.Hiscores.FirstOrDefault(x => x.MapId == mapName);
            if (scoreObj != null)
            {
                for (int i = 0; i < lb.Count; i++)
                {
                    if (scoreObj.Score > lb[i].Score)
                    {
                        lb.Remove(lb[i]);
                        lb.Add(new LeaderBoardItem(playerObject.Name, scoreObj.Score));
                        break;   
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                continue;
            }
        }
        var sorted = lb.OrderByDescending(x => x.Score).ToList();
        return sorted;
    }

    //Insert new player object to the database, if the username was valid after trying to sign up.
    public async void RegisterNewPlayer(string playerName, string pw)
    {
        try
        {
            player.Name = playerName;
            player.Password = PasswordHandler.Encrypt(pw, player.Name);
            await playerCollection.InsertOneAsync(player);
            hsCollection = player.ScoreCollection;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

    }

    //Initialize key functions after crucial operations like log in and log out
    public void ResetValues()
    {
        player = new PlayerRef();
        LoginSuccess = false;
        SignInSuccess = false;
    }

    //Create an empty list with a length of 5 empty leaderboard items.
    public void SetupLeaderboards()
    {
        lb = new List<LeaderBoardItem>();
        lb.Add(new LeaderBoardItem("", 0));
        lb.Add(new LeaderBoardItem("", 0));
        lb.Add(new LeaderBoardItem("", 0));
        lb.Add(new LeaderBoardItem("", 0));
        lb.Add(new LeaderBoardItem("", 0));
    }
}