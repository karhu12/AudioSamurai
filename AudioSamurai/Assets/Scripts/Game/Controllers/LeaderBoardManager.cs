using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardManager : Singleton<LeaderBoardManager>
{
    public string MapName { get; set; }
    public List<LeaderBoardItem> Leaderboards { get; set; }
}
