using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NormalAttack : CharacterController
{
    bool attackBuffer;

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        canAttack = false;
        canEvade = false;
        attackBuffer = false;
        animator.SetTrigger("Attack");
        float y_velocity = movement.RigidBody.velocity.y;
        movement.RigidBody.velocity = new Vector3(0f, y_velocity, 0f);
        animEvent.onAnimationStart += CleanBuffer;
        animEvent.onAnimationComplete += AttackFinished;
        animEvent.onAnimationCancelable += CanCombo;
    }

    public override void Salir()
    {
        movement.enabled = true;
        animEvent.onAnimationStart -= CleanBuffer;
        animEvent.onAnimationComplete -= AttackFinished;
        animEvent.onAnimationCancelable -= CanCombo;
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive)
        {
            animEvent.onAnimationStart += CleanBuffer;
            animEvent.onAnimationComplete += AttackFinished;
            animEvent.onAnimationCancelable -= CanCombo;
        }
    }

    private void OnDisable()
    {
        if (isActive)
        {
            animEvent.onAnimationStart -= CleanBuffer;
            animEvent.onAnimationComplete -= AttackFinished;
            animEvent.onAnimationCancelable -= CanCombo;
        }
    }

    private void CleanBuffer(CharacterAnimatorState state)
    {
        if (state == CharacterAnimatorState.ATTACK)
        {
            attackBuffer = false;
            canAttack = false;
        }
    }

    private void CanCombo(CharacterAnimatorState state)
    {
        if (state == CharacterAnimatorState.ATTACK)
        {
            canAttack = true;
        }
    }

    private void AttackFinished(CharacterAnimatorState state)
    {
        if (attackBuffer)
        {
            return;
        }
        if (state == CharacterAnimatorState.ATTACK)
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

    public override void Attack()
    {
        if(canAttack)
        {
            canAttack = false;
            animator.SetTrigger("Attack");
            attackBuffer = true;
        }
    }
}
