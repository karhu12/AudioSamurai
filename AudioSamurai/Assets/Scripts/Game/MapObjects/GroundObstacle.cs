using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundObstacle : MapObject
{

    /* Constants */
    public new const string Type = "GroundObstacle";

    public override string GetMapObjectType() {
        return GroundObstacle.Type;
    }

    override protected void OnPlayerCollision(Player player) {
        /* TODO : lose combo */
        player.TakeDamage();
        base.OnPlayerCollision(player);
    }

    protected override void OnPlayerHit(Player player) {
        /* Do nothing since it's an obstacle. */ 
    }
}
