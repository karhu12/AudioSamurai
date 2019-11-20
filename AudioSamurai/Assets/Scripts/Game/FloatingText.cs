using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : Poolable
{
    public float DestroyTime = 3f;
    public Vector3 Offset = new Vector3(0, 0, 0);

    void Start()
    {
        transform.localPosition += Offset;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(new Vector3(0, 180, 0));
        Destroy(gameObject, DestroyTime);
    }
}
