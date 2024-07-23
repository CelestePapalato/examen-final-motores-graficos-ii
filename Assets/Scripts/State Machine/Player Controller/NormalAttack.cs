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
        attackBuffer = false;
        animator.SetTrigger("Attack");
        StopPlayerMovement();
        animEvent.onAnimationStart += CleanBuffer;
        animEvent.onAnimationComplete += AttackFinished;
        animEvent.onAnimationCancelable += CanCombo;
    }

    public override void Salir()
    {
        ResumePlayerMovement();
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

    public override void Move(Vector2 input)
    {
        if (inputRelatedToCamera)
        {
            input = Quaternion.Euler(0f, 0f, -camera.transform.eulerAngles.y) * input;
        }
        movement.Direction = input;
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
