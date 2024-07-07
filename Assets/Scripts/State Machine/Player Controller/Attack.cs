using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : CharacterController
{
    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        animator.SetTrigger("Attack");
        animEvent.onAnimationComplete += AttackFinished;
    }

    public override void Salir()
    {
        movement.enabled = true;
        animEvent.onAnimationComplete -= AttackFinished;
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive)
        {

            animEvent.onAnimationComplete += AttackFinished;
        }
    }

    private void OnDisable()
    {
        if (isActive)
        {

            animEvent.onAnimationComplete -= AttackFinished;
        }
    }

    private void AttackFinished(CharacterAnimatorState state)
    {
        if(state == CharacterAnimatorState.ATTACK)
        {
            personaje.CambiarEstado(null);
        }
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
