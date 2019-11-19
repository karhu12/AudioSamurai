using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundObstacle : Obstacle
{
    /* Constants */
    public new const string Type = "GroundObstacle";

    public override string GetMapObjectType() {
        return GroundObstacle.Type;
    }
}
