using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MapObject
{
    override protected void OnPlayerCollision(Player player) {
        player.TakeDamage(GameController.Instance.SelectedSongmap.HealthDrainlevel);
        ScoreSystem.Instance.ResetCombo();
        base.OnPlayerCollision(player);
    }

    protected override void OnPlayerHit(Player player) {
        int score = GameController.Instance.CalculateHitScore(Timing);
        ScoreSystem.Instance.AddScore(score);
        base.OnPlayerHit(player);
    }

}
