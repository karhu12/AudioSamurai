using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MapObject
{
    protected void ShowScoreText(int score) {
        ScoreSystem.HitType type = (ScoreSystem.HitType)score;
        FloatingTextManager.Instance.PlaceFloatingText(transform.position, new Vector3(.5f, 1.5f, .5f),ScoreSystem.GetHitTypeString(type), ScoreSystem.GetHitTypeColor(type));
    }

    protected override void OnPlayerHit(Player player) {
        int score = GameController.Instance.CalculateHitScore(Timing);
        ShowScoreText(score);
        ScoreSystem.Instance.AddScore(score);
        player.RestoreHealth();
        ReturnToPool();
    }
}
