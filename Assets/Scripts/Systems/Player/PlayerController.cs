using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    public UnityEvent<Vector2> OnMoveInput;
    public UnityEvent OnAttackInput;
    public UnityEvent OnSpecialAttackInput;
    public UnityEvent OnInteractInput;

    private void OnMovement(InputValue inputValue)
    {
        Vector2 input = (inputValue != null) ? inputValue.Get<Vector2>() : Vector2.zero;
        OnMoveInput?.Invoke(input);
    }

    private void OnAttack()
    {
        OnAttackInput?.Invoke();
    }

    private void OnSpecialAttack()
    {
        OnSpecialAttackInput?.Invoke();
    }

    private void OnEvade()
    {
        //chara?.Evade();
    }

    private void OnInteract()
    {   
        OnInteractInput?.Invoke();
    }
}