using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MapObject
{
    /* Constants */
    public new const string Type = "GroundEnemy";

    public override string GetMapObjectType()
    {
        return GroundEnemy.Type;
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
