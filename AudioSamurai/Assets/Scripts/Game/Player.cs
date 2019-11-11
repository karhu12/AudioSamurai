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
    public const float DEFAULT_SPEED = 10f;

    public const string COLLIDER_NAME = "Player";
    public const string HIT_COLLIDER_NAME = "HitArea";

    public Collider hitCollider;

    private IEnumerator jumpAttack;
    private IEnumerator attack;
    private float beatDuration = 0;


    public bool IsAttacking { get; private set; }
    public bool IsJumpAttacking { get; private set; }
    public bool IsRunning { get; set; }

    private void Awake()
    {
        hitCollider.gameObject.SetActive(false);
        IsAttacking = false;
        IsJumpAttacking = false;
        IsRunning = false;
    }

    private void Update()
    {
        CheckPlayerInput();
    }

    private void FixedUpdate()
    {
        //Add direction and velocity to player character depending on a song bpm
        if (IsRunning)
        {
            transform.position += new Vector3(0f, 0f, beatDuration * Time.deltaTime * DEFAULT_SPEED);
        }
    }

    public void ChangeSpeed(float bpm)
    {
        if (bpm > 0)
        {
            beatDuration = 60 / bpm;
        }
    }

    public void Attack()
    {
        attack = AttackCoroutine();
        StartCoroutine(attack);
    }

    public void JumpAttack()
    {
        jumpAttack = JumpAttackCoroutine();
        StartCoroutine(jumpAttack);
    }
    IEnumerator AttackCoroutine()
    {
        if (jumpAttack != null)
        {
            StopCoroutine(jumpAttack);
            IsJumpAttacking = false;
        }

        IsAttacking = true;
        hitCollider.gameObject.SetActive(true);
        float addAmount = -.3f;
        while (transform.position.y > GROUND_PLACEMENT)
        {
            yield return new WaitForSeconds(0.001f);
            if (transform.position.y + addAmount <= GROUND_PLACEMENT)
            {
                transform.position = new Vector3(transform.position.x, GROUND_PLACEMENT, transform.position.z);
            }
            else
            {
                transform.position += new Vector3(0, addAmount, 0);
            }
        }
        yield return new WaitForSeconds(ATTACK_TIME);
        IsAttacking = false;
        hitCollider.gameObject.SetActive(false);
    }

    IEnumerator JumpAttackCoroutine()
    {
        if (attack != null)
        {
            StopCoroutine(attack);
            IsAttacking = false;
        }

        IsJumpAttacking = true;
        hitCollider.gameObject.SetActive(true);
        float addAmount = .3f;
        while (transform.position.y < AIR_PLACEMENT)
        {
            yield return new WaitForSeconds(0.001f);
            if (transform.position.y + addAmount >= AIR_PLACEMENT)
            {
                transform.position = new Vector3(transform.position.x, AIR_PLACEMENT, transform.position.z);
            }
            else
            {
                transform.position += new Vector3(0, addAmount, 0);
            }
        }
        yield return new WaitForSeconds(ATTACK_TIME);
        IsJumpAttacking = false;
        hitCollider.gameObject.SetActive(false);
    }

    private void CheckPlayerInput()
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
}
