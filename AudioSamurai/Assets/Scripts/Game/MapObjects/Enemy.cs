using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MapObject
{
    public bool HasBeenHit { get; protected set; } = false;
    public bool HasBeenMissed { get; protected set; } = false;

    protected void ShowScoreText(int score) {
        ScoreSystem.HitType type = (ScoreSystem.HitType)score;
        FloatingTextManager.Instance.PlaceFloatingText(transform.position, new Vector3(1f, 1.5f, .5f),ScoreSystem.GetHitTypeString(type), ScoreSystem.GetHitTypeColor(type));
    }

    protected override void OnEnemyMiss(Player player)
    {
        if (!HasHadCollision) {
            if (!HasBeenMissed) {
                HasBeenMissed = true;
                ScoreSystem.Instance.Miss();
            }
        }
    }

    protected override void OnPlayerCollision(Player player) {
        if (!HasHadCollision) {
            HasHadCollision = true;
            float damage = player.TakeDamage(GameController.Instance.SelectedSongmap.HealthDrainlevel);
            FloatingTextManager.Instance.PlaceFloatingText(player.transform.position, new Vector3(.5f, 2.5f, .5f), $"-{damage}", Color.red);
            if (!HasBeenMissed) {
                ScoreSystem.Instance.Miss();
            }
        }
    }

    protected override void OnPlayerHit(Player player) {
        float hitTime = SongmapController.Instance.GetAccuratePlaybackPositionMs();
        if (!HasBeenHit) {
            HasBeenHit = true;
            int score = GameController.Instance.CalculateHitScore(Timing, hitTime);
            FindObjectOfType<AudioManager>().Play("PlayerAttack");
            ShowScoreText(score);
            ScoreSystem.Instance.AddScore(score);
            if ((ScoreSystem.HitType)score == ScoreSystem.HitType.Miss) {
                ScoreSystem.Instance.ResetCombo();
            } else {
                player.RestoreHealth();
                ReturnToPool();
            }
        }
    }

    public override void ReturnToPool() {
        HasBeenHit = false;
        HasBeenMissed = false;
        base.ReturnToPool();
    }

}
