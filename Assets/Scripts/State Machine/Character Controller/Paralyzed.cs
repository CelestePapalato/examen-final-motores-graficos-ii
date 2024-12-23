using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paralyzed : CharacterState
{
    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        StopPlayerActions();
        EnableAgent(false);
        EnableRigidbody(true);
        //currentCharacter.MovementComponent.enabled = false;
        currentCharacter.MovementComponent.UpdateRotationON = false;
        currentCharacter.MovementComponent.UpdatePositionON = false;
        currentCharacter.AnimationEventHandler.onAnimationComplete += StunFinished;
    }

    public override void Salir()
    {
        ResumePlayerMovement();
        currentCharacter.AnimationEventHandler.onAnimationComplete -= StunFinished;
        currentCharacter.MovementComponent.UpdateRotationON = true;
        currentCharacter.MovementComponent.UpdatePositionON = true;
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive)
        {
            currentCharacter.AnimationEventHandler.onAnimationComplete += StunFinished;
        }
    }

    private void OnDisable()
    {
        if (isActive)
        {
            currentCharacter.AnimationEventHandler.onAnimationComplete -= StunFinished;
        }
    }


    private void StunFinished(CharacterAnimatorState state)
    {
        if (state == CharacterAnimatorState.STUN || state == CharacterAnimatorState.INTERACTION)
        {
            personaje.CambiarEstado(null);
        }
    }
    
    public override void Attack() { }
}
