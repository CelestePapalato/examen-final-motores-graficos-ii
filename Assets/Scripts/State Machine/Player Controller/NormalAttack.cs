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
        currentCharacter.Animator?.SetTrigger("Attack");
        StopPlayerMovement();
        currentCharacter.AnimationEventHandler.onAnimationStart += CleanBuffer;
        currentCharacter.AnimationEventHandler.onAnimationComplete += AttackFinished;
        currentCharacter.AnimationEventHandler.onAnimationCancelable += CanCombo;
    }

    public override void Salir()
    {
        ResumePlayerMovement();
        currentCharacter.AnimationEventHandler.onAnimationStart -= CleanBuffer;
        currentCharacter.AnimationEventHandler.onAnimationComplete -= AttackFinished;
        currentCharacter.AnimationEventHandler.onAnimationCancelable -= CanCombo;
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart += CleanBuffer;
            currentCharacter.AnimationEventHandler.onAnimationComplete += AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable -= CanCombo;
        }
    }

    private void OnDisable()
    {
        if (isActive)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart -= CleanBuffer;
            currentCharacter.AnimationEventHandler.onAnimationComplete -= AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable -= CanCombo;
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
        currentCharacter.MovementComponent.Direction = input;
    }

    public override void Attack()
    {
        if(canAttack)
        {
            canAttack = false;
            currentCharacter.Animator?.SetTrigger("Attack");
            attackBuffer = true;
        }
    }
}
