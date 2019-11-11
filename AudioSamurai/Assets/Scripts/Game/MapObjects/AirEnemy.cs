using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : MapObject
{
    /* Constants */
    public new const string Type = "AirEnemy";

    public override string GetMapObjectType()
    {
        return AirEnemy.Type;
    }

    public override VerticalPlacement Placement
    {
        get => VerticalPlacement.Air;
    }

    override protected void OnPlayerCollision(Player player)
    {
        /* TODO : Player take damage + lose combo */
    }

    protected override void OnPlayerHit(Player player)
    {
        /* TODO : Add combo to player and destroy self */
        base.OnPlayerHit(player);
    }
}
