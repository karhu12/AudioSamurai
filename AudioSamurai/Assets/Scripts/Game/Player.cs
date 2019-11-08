using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    /* Constants */
    public const float GROUND_PLACEMENT = .05f;
    public const float AIR_PLACEMENT = 2.05f;
    public const float ATTACK_TIME = 0.1f;

    public Collider hitCollider;

    private IEnumerator jumpAttack;
    private IEnumerator attack;

    public const string COLLIDER_NAME = "Player";
    public const string HIT_COLLIDER_NAME = "HitArea";


    public bool IsAttacking { get; private set; }
    public bool IsJumpAttacking { get; private set; }

    private void Awake()
    {
        hitCollider.gameObject.SetActive(false);
        hitCollider.transform.position = new Vector3(0, 1, GameController.BEAT_DISTANCE);
        IsAttacking = false;
        IsJumpAttacking = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Attack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            JumpAttack();
        }
    }

    public void Attack()
    {
        attack = AttackCoroutine();
        StartCoroutine(attack);
    }

    IEnumerator AttackCoroutine()
    {
        if (jumpAttack != null)
            StopCoroutine(jumpAttack);

        IsAttacking = true;
        hitCollider.gameObject.SetActive(true);
        /* TODO : Implement real player movement using rigidbody forces */
        transform.position = new Vector3(transform.position.x, GROUND_PLACEMENT, transform.position.z);
        yield return new WaitForSeconds(ATTACK_TIME);
        IsAttacking = false;
        hitCollider.gameObject.SetActive(false);
    }

    public void JumpAttack()
    {
        jumpAttack = JumpAttackCoroutine();
        StartCoroutine(jumpAttack);
    }

    IEnumerator JumpAttackCoroutine()
    {
        if (attack != null)
            StopCoroutine(attack);

        IsJumpAttacking = true;
        hitCollider.gameObject.SetActive(true);
        /* TODO : Implement real player movement using rigidbody forces */
        transform.position = new Vector3(transform.position.x, AIR_PLACEMENT, transform.position.z);
        yield return new WaitForSeconds(ATTACK_TIME);
        IsJumpAttacking = false;
        hitCollider.gameObject.SetActive(false);
    }
}
