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
    public const string GROUND_TAG = "Ground";

    public Collider hitCollider;

    private IEnumerator jumpAttack;
    private IEnumerator attack;
    private Rigidbody rb;
    private float beatDuration;


    public bool IsAttacking { get; private set; }
    public bool IsJumpAttacking { get; private set; }
    public bool OnGround { get; private set; }
    public bool IsRunning { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hitCollider.gameObject.SetActive(false);
        IsAttacking = false;
        IsJumpAttacking = false;
        IsRunning = true;
        OnGround = true;
        ChangeSpeed(120);
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
            rb.AddForce(new Vector3(0f, 0f, beatDuration * Time.deltaTime * 2), ForceMode.Impulse);
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

    IEnumerator AttackCoroutine()
    {
        if (jumpAttack != null)
            StopCoroutine(jumpAttack);

        IsAttacking = true;
        hitCollider.gameObject.SetActive(true);
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * DEFAULT_SPEED;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * DEFAULT_SPEED;

        transform.Translate(horizontal, 0, vertical);
        rb.AddForce(new Vector3(0, -5, 0), ForceMode.Impulse);
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
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * DEFAULT_SPEED;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * DEFAULT_SPEED;

        transform.Translate(horizontal, 0, vertical);
        rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
        OnGround = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == GROUND_TAG)
        {
            OnGround = true;
        }
    }
}
