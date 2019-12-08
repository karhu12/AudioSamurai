using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy
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
}
