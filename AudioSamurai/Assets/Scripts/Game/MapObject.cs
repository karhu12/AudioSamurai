using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    /* Constants */
    public const float AIR_PLACEMENT = 3.05f;
    public const float GROUND_PLACEMENT = .55f;
    public const float INVALID_PLACEMENT = -1f;

    public enum VerticalPlacement
    {
        Air,
        Ground
    }

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

    /* returns the map objects own object type. Should be overridden in derived classes to represent type. */
    protected virtual string GetMapObjectType()
    {
        return "GenericMapObject";
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
        }
    }

    /*
     * Triggered when the MapObject has collision with the player.
     * Should be overridden in derived classes to implement logic.
     */
    protected virtual void OnPlayerCollision(Player player)
    {

    }


    /*
     * Triggered when the MapObject has collision with the players ground hit collider. (Enabled when attacking in air or ground)
     * Should be overridden in derived classes to implement logic.
     */
    protected virtual void OnPlayerHit(Player player)
    {

    }
}

public class GroundEnemy : MapObject
{
    override protected string GetMapObjectType()
    {
        return "GroundEnemy";
    }

    override protected void OnPlayerCollision(Player player)
    {
        /* TODO : Player take damage + lose combo */
        //Reset the combo back to default/minimum.
        ScoreSystem.combo = ScoreSystem.MIN_COMBO;
    }

    protected override void OnPlayerHit(Player player)
    {
        /* TODO : Add combo to player and destroy self */
        //Check whether the combo is or isn't in its maximum value and then make different score operations based on that. 
        if (ScoreSystem.combo < ScoreSystem.MAX_COMBO)
        {
            ScoreSystem.combo *= 2;
            ScoreSystem.score += 10 * ScoreSystem.combo;
        }
        else if (ScoreSystem.combo == ScoreSystem.MAX_COMBO)
        {
            ScoreSystem.score += 15 * ScoreSystem.combo;
        }
    }
}

public class AirEnemy : MapObject
{
    public override VerticalPlacement Placement
    {
        get => VerticalPlacement.Air;
    }

    override protected string GetMapObjectType()
    {
        return "AirEnemy";
    }

    override protected void OnPlayerCollision(Player player)
    {
        /* TODO : Player take damage + lose combo */
        //Reset the combo back to default/minimum.
        ScoreSystem.combo = ScoreSystem.MIN_COMBO;
    }

    protected override void OnPlayerHit(Player player)
    {
        /* TODO : Add combo to player and destroy self */
        //Check whether the combo is or isn't in its maximum value and then make different score operations based on that.   
        if (ScoreSystem.combo < ScoreSystem.MAX_COMBO)
        {
            ScoreSystem.combo *= 2;
            ScoreSystem.score += 15 * ScoreSystem.combo;
        }
        else if(ScoreSystem.combo == ScoreSystem.MAX_COMBO)
        {
            ScoreSystem.score += 15 * ScoreSystem.combo;
        }
    }
}
