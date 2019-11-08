using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackTester : MonoBehaviour
{
    // Start is called before the first frame update
    //public InputMaster inputMaster;

    public InputActionAsset inputActionAsset;

    InputAction action;

    private void Awake()
    {
        var inputActions = inputActionAsset.FindActionMap("Player");
        action = inputActions.FindAction("Attack");
        inputActionAsset.Enable();
    }

    private void OnEnable()
    {
        /*_controls.Player.Attack.performed += HandleAttack;
        _controls.Player.Enable();*/

        action.started += OnStarted;
        action.performed += OnPerformed;
        //action.canceled += OnCancelled;
    }

    /*private void OnCancelled(InputAction.CallbackContext obj)
    {
        Debug.Log("cancelled");
    }*/

    private void OnPerformed(InputAction.CallbackContext obj)
    {
        Debug.Log("performed");
    }

    private void OnStarted(InputAction.CallbackContext obj)
    {
        Debug.Log("started");
    }

    private void OnDisable()
    {
        action.started -= OnStarted;
        action.performed -= OnPerformed;
        //action.canceled -= OnCancelled;
        /*_controls.Player.Attack.performed -= HandleAttack;
        _controls.Player.Attack.Disable();*/
    }



   /* private void HandleAttack(InputAction.CallbackContext context)
    {
        Debug.Log("asd");
    }*/
}
