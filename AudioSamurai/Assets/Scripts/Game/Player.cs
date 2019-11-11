﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Player : MonoBehaviour
{
    /* Constants */
    public const float GROUND_PLACEMENT = 0f;
    public const float AIR_PLACEMENT = 2f;
    public const float ATTACK_TIME = 0.1f;
    public const float DEFAULT_SPEED = 10f;

    public const string COLLIDER_NAME = "Player";
    public const string HIT_COLLIDER_NAME = "HitArea";

    public Collider hitCollider;

    /* Cosmetic */
    public GameObject hatModel;
    public GameObject swordModel;

    public Equipment Equipment { get; private set; }

    private IEnumerator jumpAttack;
    private IEnumerator attack;
    private float beatDuration = 0;
    private Animator animator;


    public bool IsAttacking { get; private set; }
    public bool IsJumpAttacking { get; private set; }
    public bool IsRunning { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitCollider.gameObject.SetActive(false);
        IsAttacking = false;
        IsJumpAttacking = false;
        IsRunning = false;
        Equipment = new Equipment(gameObject);
        StartCoroutine(EquipCoroutine()); 
    }

    /* Performs item equipping. NOTE : For some reason equipment is invisible after equipping if coroutine is not used to yield result after equip.*/
    private IEnumerator EquipCoroutine()
    {
        if (hatModel != null)
            Equipment.Equip(hatModel, "Hat");

        yield return null;

        if (swordModel != null)
            Equipment.Equip(swordModel, "Katana");

        yield return null;
    }

    private void Update()
    {
        animator.SetBool("IsRunning", IsRunning);
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsJumpAttacking", IsJumpAttacking);
        animator.SetFloat("Y", transform.position.y);
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

    /* 
     * Performs an normal attack which is ran in an coroutine because it has time variables.
     * Attack enables HitArea collider which is triggered on MapObjects if they collide.
     */
    public void Attack()
    {
        attack = AttackCoroutine();
        StartCoroutine(attack);
    }

     /*
     * Works the same as normal attack but IsAttacking and IsJumpAttacking should be checked respectively.
     * Performs an jump attack which is ran in an coroutine because it has time variables. 
     */
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
        float addAmount = -.15f;
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
        float addAmount = .15f;
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