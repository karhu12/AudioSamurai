using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MapObject
{
    public bool HasBeenHit { get; private set; } = false;

    protected void ShowScoreText(int score) {
        ScoreSystem.HitType type = (ScoreSystem.HitType)score;
        FloatingTextManager.Instance.PlaceFloatingText(transform.position, new Vector3(.5f, 1.5f, .5f),ScoreSystem.GetHitTypeString(type), ScoreSystem.GetHitTypeColor(type));
    }

    protected override void OnEnemyMiss(Player player)
    {
        ScoreSystem.Instance.ResetCombo();
        GameData.Instance.OnHitMissed();
        base.OnEnemyMiss(player);
    }

    protected override void OnPlayerHit(Player player) {
        GameData.Instance.OnSuccessfulHit();
        if (!HasBeenHit) {
            HasBeenHit = true;
            int score = GameController.Instance.CalculateHitScore(Timing);
            ShowScoreText(score);
            ScoreSystem.Instance.AddScore(score);
            if ((ScoreSystem.HitType)score == ScoreSystem.HitType.Miss) {
                ScoreSystem.Instance.ResetCombo();
            }
            else {
                player.RestoreHealth();
                ReturnToPool();
            }
        }
    }

    public override void ReturnToPool() {
        HasBeenHit = false;
        base.ReturnToPool();
    }

}
