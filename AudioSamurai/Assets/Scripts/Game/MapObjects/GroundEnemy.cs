using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : Enemy
{
    /* Constants */
    public new const string Type = "GroundEnemy";

    private void Awake() {
        OverheadPosition = 2.5f;
    }

    public override string GetMapObjectType()
    {
        return GroundEnemy.Type;
    }
}
