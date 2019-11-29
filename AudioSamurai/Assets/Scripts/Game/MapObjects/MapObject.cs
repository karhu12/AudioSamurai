using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapObject : Poolable
{
    /* Constants */
    public const float AIR_PLACEMENT = 3.05f;
    public const float GROUND_PLACEMENT = 0f;
    public const float INVALID_PLACEMENT = -1f;

    // Constant used for determining the type of object. Mainly used for helping on instantiation. Sub classes should declare similiar one.
    public const string Type = "MapObject";

    /* returns the map objects own object type. Should be overridden in derived classes to represent type. */
    public virtual string GetMapObjectType()
    {
        return MapObject.Type;
    }

    public enum VerticalPlacement
    {
        Air,
        Ground
    }

    public float Timing { get; set; }
    public bool HasHadCollision { get; protected set; } = false;

    /* Placement constant of class. Must be overridden in derived class to change verticla placement! */
    public virtual VerticalPlacement Placement 
    {
        get => VerticalPlacement.Ground;
    }

    /* Returns the value of current vertical placement as float. */
    public float GetPlacementValue()
    {
        switch (Placement)
        {
            case VerticalPlacement.Ground:
                return GROUND_PLACEMENT;
            case VerticalPlacement.Air:
                return AIR_PLACEMENT;
        }
        return INVALID_PLACEMENT;
    }

    /* Handle collisions with player and his attacks */
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case Player.COLLIDER_NAME:
                OnPlayerCollision(FindObjectOfType<Player>());
                break;
            case Player.HIT_COLLIDER_NAME:
                OnPlayerHit(FindObjectOfType<Player>());
                break;
            case EnemyChecker.COLLIDER_NAME:
                OnEnemyMiss(FindObjectOfType<Player>());
                break;
            case KillPlane.COLLIDER_NAME:
                ReturnToPool();
                break;
        }
    }

    /*
     * Triggered when the MapObject has collision with the player.
     * Should be overridden in derived classes to implement logic.
     * NOTE : It is currently possible for enemy to have collision with player multiple times. Could be fixed with collision times counter.
     */
    protected virtual void OnPlayerCollision(Player player)
    {
        // Debug.Log($"Player Collision at: {SongmapController.Instance.AudioSource.time}");
        if (!HasHadCollision) {
            HasHadCollision = true;
            float damage = player.TakeDamage(GameController.Instance.SelectedSongmap.HealthDrainlevel);
            FloatingTextManager.Instance.PlaceFloatingText(player.transform.position, new Vector3(.5f, 2.5f, .5f), $"-{damage}", Color.red);
            ScoreSystem.Instance.ResetCombo();
        }
    }

    /*
     *Triggered when player passes enemy on wrong Y-axel level
     */
    protected virtual void OnEnemyMiss(Player player)
    {

    }

    /*
     * Triggered when the MapObject has collision with the players ground hit collider. (Enabled when attacking in air or ground)
     * Should be overridden in derived classes to implement logic.
     */
    protected virtual void OnPlayerHit(Player player)
    {
        // Debug.Log($"HitArea Collision at: {SongmapController.Instance.AudioSource.time}");
    }

    protected virtual IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1);
    }

    public override void ReturnToPool() {
        HasHadCollision = false;
        base.ReturnToPool();
    }

}
