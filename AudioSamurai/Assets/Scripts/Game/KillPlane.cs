using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public const string COLLIDER_NAME = "KillPlane";
    public GameObject followTarget;
    public Vector3 followOffset;
    public Quaternion rotation;

    public bool ignoreTargetX = false;
    public bool ignoreTargetY = false;
    public bool ignoreTargetZ = false;


    public void Update()
    {
        if (followTarget != null)
        {
            float targetX = (ignoreTargetX ? 0 : followTarget.transform.position.x);
            float targetY = (ignoreTargetY ? 0 : followTarget.transform.position.y);
            float targetZ = (ignoreTargetZ ? 0 : followTarget.transform.position.z);
            this.transform.position = followOffset + new Vector3(targetX, targetY, targetZ);
        }
    }
}
