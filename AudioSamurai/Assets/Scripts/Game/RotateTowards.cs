using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public GameObject target;
    public Vector3 eulers;

    void Update()
    {
        transform.LookAt(target.transform);
        transform.Rotate(eulers);
    }
}
