using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirObstacle : Obstacle
{
    /* Constants */
    public new const string Type = "AirObstacle";

    public override VerticalPlacement Placement
    {
        get => VerticalPlacement.Air;
    }
    public override string GetMapObjectType() {
        return AirObstacle.Type;
    }
}
