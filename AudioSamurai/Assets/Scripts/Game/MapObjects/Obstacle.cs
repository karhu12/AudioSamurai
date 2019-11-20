using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MapObject
{
    protected override void OnPlayerHit(Player player) {
        /* Do nothing since it's an obstacle. */
    }
}
