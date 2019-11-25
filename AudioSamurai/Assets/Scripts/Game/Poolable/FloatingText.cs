using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : Poolable
{
    public float DestroyTime = 1f;
    private Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        StartCoroutine(ReturnToPoolCoroutine());
    }

    private IEnumerator ReturnToPoolCoroutine()
    {
        do {
            yield return null;
        } while (AnimatorIsPlaying());
        ReturnToPool();
    }
    private bool AnimatorIsPlaying() {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
