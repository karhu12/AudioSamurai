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
        player.TakeDamage();
        ScoreSystem.Instance.ResetCombo();
        GameData.Instance.OnHitMissed();
        base.OnPlayerCollision(player);
    }

    protected override void OnPlayerHit(Player player)
    {
        /* TODO : Add combo to player and destroy self */
        ScoreSystem.Instance.AddCombo();
        GameData.Instance.OnSuccessfulHit();
        base.OnPlayerHit(player);
    }
}
