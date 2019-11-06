using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class AttackTester : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private InputMaster _controls;

    private void OnEnable()
    {
        _controls.Player.Attack.performed += HandleAttack;
    }

    private void OnDisable()
    {
        _controls.Player.Attack.performed -= HandleAttack;
    }

    private void HandleAttack(InputAction.CallbackContext context)
    {
        Debug.Log("asd");
    }
}
