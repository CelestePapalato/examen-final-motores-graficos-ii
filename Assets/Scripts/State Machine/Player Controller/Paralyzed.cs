using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paralyzed : CharacterController
{
    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        StopPlayerMovement();
        movement.enabled = false;
        animEvent.onAnimationComplete += StunFinished;
    }

    public override void Salir()
    {
        ResumePlayerMovement();
        animEvent.onAnimationComplete -= StunFinished;
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive)
        {
            animEvent.onAnimationComplete += StunFinished;
        }
    }

    private void OnDisable()
    {
        if (isActive)
        {
            animEvent.onAnimationComplete -= StunFinished;
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
