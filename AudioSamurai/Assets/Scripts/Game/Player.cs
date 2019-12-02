using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /* Constants */
    public const float GROUND_PLACEMENT = 0f;
    public const float AIR_PLACEMENT = 2f;
    public const float ATTACK_TIME = 0.1f;
    public const float DEFAULT_SPEED = 10f;
    public const float HIT_AREA_OFFSET = 1.75f;
    public const float LOCAL_HIT_AREA_OFFSET = 0f;
    public const float HIT_AREA_DEPTH = .5f;
    public const float STARTING_HEALTH = 100;
    public const float DAMAGE_AMOUNT = 3;
    public const float HEALTH_RESTORE_AMOUNT = 2;

    public const string COLLIDER_NAME = "Player";
    public const string HIT_COLLIDER_NAME = "HitArea";
    public const string INPUT_MAP = "Player";
    public const string ATTACK_ACTION = "Attack";
    public const string JUMP_ATTACK_ACTION = "Jump Attack";
    public const string ALT_ATTACK_ACTION = "Attack Alt";
    public const string ALT_JUMP_ATTACK_ACTION = "Jump Attack Alt";
    public const string PARRY_ACTION  = "Parry";

    public Collider hitCollider;
    public InputActionAsset inputActionAsset;
    public HitIndicator airHitIndicator;
    public HitIndicator groundHitIndicator;

    /* Cosmetic */
    public GameObject hatModel;
    public GameObject swordModel;
    public GameObject healthBarControl;
    public GameObject modMenu;
   

    public Equipment Equipment { get; private set; }

    private IEnumerator jumpAttack;
    private IEnumerator attack;
    private float currentBpm = 0;
    private float lastMovementTime = 0;
    private Animator animator;
    private HealthBarController hbc;
    private InputAction playerAttack;
    private InputAction playerAltAttack;
    private InputAction playerJumpAttack;
    private InputAction playerAltJumpAttack;

    public bool IsAttacking { get; private set; }
    public bool IsJumpAttacking { get; private set; }
    public bool IsRunning { get; set; }
    public float Health { get; private set; }

    private void Awake()
    {
        var inputActions = inputActionAsset.FindActionMap(Player.INPUT_MAP);
        playerAttack = inputActions.FindAction(Player.ATTACK_ACTION);
        playerJumpAttack = inputActions.FindAction(Player.JUMP_ATTACK_ACTION);
        playerAltAttack = inputActions.FindAction(Player.ALT_ATTACK_ACTION);
        playerAltJumpAttack = inputActions.FindAction(Player.ALT_JUMP_ATTACK_ACTION);
        inputActionAsset.Enable();
        animator = GetComponent<Animator>();
        hbc = healthBarControl.GetComponent<HealthBarController>();
        hitCollider.gameObject.SetActive(false);
        hitCollider.transform.position = new Vector3(0, 1, HIT_AREA_OFFSET + LOCAL_HIT_AREA_OFFSET);
        IsAttacking = false;
        IsJumpAttacking = false;
        IsRunning = false;
        Equipment = new Equipment(gameObject);
        StartCoroutine(EquipCoroutine());
        Health = STARTING_HEALTH;
    }

    /* Makes the player take constant amount of damage multiplied by the given multiplier */
    public float TakeDamage(float damageMultiplier = 1) {
        float damage = DAMAGE_AMOUNT * damageMultiplier;
        if (Health < damage)
            damage = Health;
        Health -= damage;
        hbc.TakeDamageEffect(STARTING_HEALTH, Health);
        /* TODO : Play damage taken sound + animation? */
        return damage;
    }

    public void ResetPlayerStatus()
    {
        IsRunning = false;
        lastMovementTime = 0;
        transform.position = GameController.START_POSITION;
        RestoreHealth(true);
    }

    /* Restores players health by constant amount. Should be called when striking enemies. */
    public void RestoreHealth(bool toFull = false) {
        if (toFull) {
            Health = STARTING_HEALTH;
            hbc.HealEffect(STARTING_HEALTH, Health);
        } else {
            if (Health + HEALTH_RESTORE_AMOUNT > STARTING_HEALTH) {
                Health = STARTING_HEALTH;
            }
            else {
                Health += HEALTH_RESTORE_AMOUNT;
                hbc.HealEffect(STARTING_HEALTH, Health);
            }
        }
    }
    
    /* Changes player speed to match given BPM */
    public void ChangeSpeed(float bpm)
    {
        if (bpm > 0)
        {
            currentBpm = bpm;
        }
    }

    /* 
     * Performs an normal attack which is ran in an coroutine because it has time variables.
     * Attack enables HitArea collider which is triggered on MapObjects if they collide.
     */
    public void Attack(InputAction.CallbackContext obj)
    {
        if (IsRunning)
        {
            attack = AttackCoroutine();
            StartCoroutine(attack);
        }
    }

     /*
     * Works the same as normal attack but IsAttacking and IsJumpAttacking should be checked respectively.
     * Performs an jump attack which is ran in an coroutine because it has time variables. 
     */
    public void JumpAttack(InputAction.CallbackContext obj)
    {
        if (IsRunning)
        {
            jumpAttack = JumpAttackCoroutine();
            StartCoroutine(jumpAttack);
        }
    }

    /* Private methods */

    private void FixedUpdate() {
        animator.SetBool("IsRunning", IsRunning);
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsJumpAttacking", IsJumpAttacking);
        animator.SetFloat("Y", transform.position.y);

        //Add direction and velocity to player character depending on a song bpm
        if (IsRunning) {
            float msPos = SongmapController.Instance.GetAccuratePlaybackPosition();
            float elapsed = msPos - lastMovementTime;
            transform.Translate(new Vector3(0f, 0f, GameController.BEAT_DISTANCE * (elapsed / (60 / currentBpm))));
            lastMovementTime = msPos;
        }
    }
    private void OnEnable() {
        playerAttack.performed += Attack;
        playerAltAttack.performed += Attack;
        playerJumpAttack.performed += JumpAttack;
        playerAltJumpAttack.performed += JumpAttack;
    }

    private void OnDisable() {
        playerAttack.performed -= Attack;
        playerAltAttack.performed -= Attack;
        playerJumpAttack.performed -= JumpAttack;
        playerAltJumpAttack.performed -= JumpAttack;
    }

    /* Performs item equipping. NOTE : For some reason equipment is invisible after equipping if coroutine is not used to yield result after equip.*/
    private IEnumerator EquipCoroutine() {
        if (hatModel != null)
            Equipment.Equip(hatModel, "Hat");

        yield return null;

        if (swordModel != null)
            Equipment.Equip(swordModel, "Katana");

        yield return null;
    }

    IEnumerator AttackCoroutine()
    {
        if (jumpAttack != null)
        {
            StopCoroutine(jumpAttack);
            IsJumpAttacking = false;
            hitCollider.gameObject.SetActive(false);
        }

        IsAttacking = true;

        if (groundHitIndicator != null)
            groundHitIndicator.Pop();
        /*
        while (transform.position.y > GROUND_PLACEMENT)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, GROUND_PLACEMENT, transform.position.z), 0.5f);
        }*/
        transform.position = new Vector3(transform.position.x, GROUND_PLACEMENT, transform.position.z);
        hitCollider.gameObject.SetActive(true);
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
            hitCollider.gameObject.SetActive(false);
        }

        IsJumpAttacking = true;

        if (airHitIndicator != null)
            airHitIndicator.Pop();
        /*
        while (transform.position.y < AIR_PLACEMENT)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, AIR_PLACEMENT, transform.position.z), 0.5f);
        }*/
        transform.position = new Vector3(transform.position.x, AIR_PLACEMENT, transform.position.z);
        hitCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(ATTACK_TIME);
        IsJumpAttacking = false;
        hitCollider.gameObject.SetActive(false);
    }
}

/* 
 * Class that holds references to the game objects worn by the parent of equipment.
 * Equipment works using Stitcher which stitches given equipments metarig to the metarig (bone armature) of parent object.
 * You can equipt items to different slots given by a string.
 */
public class Equipment
{
    private GameObject parent;
    private Dictionary<string, GameObject> equipment = new Dictionary<string, GameObject>();
    private Stitcher stitcher = new Stitcher();

    public Equipment(GameObject parent)
    {
        this.parent = parent;
    }

    public bool Equip(GameObject obj, string slot)
    {
        if (obj == null)
            return false;

        if (equipment.ContainsKey(slot))
        {
            GameObject worn = equipment[slot];
            MonoBehaviour.Destroy(worn);
            equipment.Remove(slot);
        }

        GameObject newEquipment = MonoBehaviour.Instantiate(obj);
        stitcher.Stitch(newEquipment, parent);
        equipment[slot] = newEquipment;
        return true;
    }

    public bool Unequip(string slot)
    {
        if (equipment.ContainsKey(slot))
        {
            GameObject worn = equipment[slot];
            MonoBehaviour.Destroy(worn);
            equipment.Remove(slot);
            return true;
        }
        return false;
    }
}