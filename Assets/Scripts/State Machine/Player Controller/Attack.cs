using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : CharacterController
{
    [SerializeField]
    string animationStateRoute = "Base Layer/Attack";

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        animator.SetTrigger("Attack");
    }

    public override void Salir()
    {
        movement.enabled = true;
        base.Salir();
    }

    public override void Move(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        if (inputRelatedToCamera)
        {
            input = Quaternion.Euler(0f, 0f, -camera.transform.eulerAngles.y) * input;
        }
        movement.Direction = input;
        Debug.Log(input);
    }
}
