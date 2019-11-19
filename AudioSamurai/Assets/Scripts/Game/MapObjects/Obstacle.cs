using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MapObject
{
    override protected void OnPlayerCollision(Player player) {
        ScoreSystem.Instance.ResetCombo();
        player.TakeDamage();
        GameData.Instance.OnHitMissed();
        base.OnPlayerCollision(player);
    }

    protected override void OnPlayerHit(Player player) {
        /* Do nothing since it's an obstacle. */
    }
}
