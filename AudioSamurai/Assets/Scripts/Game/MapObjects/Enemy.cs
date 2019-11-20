using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MapObject
{
    public GameObject floatingTextPrefab;

    protected void ShowScoreText(int score) {
        if (floatingTextPrefab != null) {
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            var text = textObj.GetComponent<TMPro.TextMeshPro>();
            switch ((ScoreSystem.HitType)score) {
                case ScoreSystem.HitType.Perfect:
                    text.color = Color.blue;
                    text.SetText(ScoreSystem.PERFECT_TEXT);
                    break;
                case ScoreSystem.HitType.Normal:
                    text.color = Color.green;
                    text.SetText(ScoreSystem.NORMAL_TEXt);
                    break;
                case ScoreSystem.HitType.Poor:
                    text.color = Color.yellow;
                    text.SetText(ScoreSystem.POOR_TEXT);
                    break;
                case ScoreSystem.HitType.Miss:
                    text.color = Color.red;
                    text.SetText(ScoreSystem.MISS_TEXT);
                    break;
            }
        }
    }

    override protected void OnPlayerCollision(Player player) {
        player.TakeDamage(GameController.Instance.SelectedSongmap.HealthDrainlevel);
        ScoreSystem.Instance.ResetCombo();
    }

    protected override void OnPlayerHit(Player player) {
        int score = GameController.Instance.CalculateHitScore(Timing);
        ShowScoreText(score);
        ScoreSystem.Instance.AddScore(score);
        player.RestoreHealth();
        ReturnToPool();
    }
}
