using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirObstacle : MapObject
{
    /* Constants */
    public new const string Type = "AirObstacle";

    public override VerticalPlacement Placement
    {
        get => VerticalPlacement.Air;
    }
    public override string GetMapObjectType() {
        return AirObstacle.Type;
    }

    override protected void OnPlayerCollision(Player player) {
        /* TODO : lose combo */
        ScoreSystem.Instance.ResetCombo();
        GameData.Instance.OnHitMissed();
        player.TakeDamage();
        base.OnPlayerCollision(player);
    }

    protected override void OnPlayerHit(Player player) {
        /* Do nothing since it's an obstacle. */
    }
}
