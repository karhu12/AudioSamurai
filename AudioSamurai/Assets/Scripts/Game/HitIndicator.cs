using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    public const string POP_TRIGGER = "Pop";
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    public void Pop()
    {
        animator.SetTrigger(POP_TRIGGER);
    }
}
