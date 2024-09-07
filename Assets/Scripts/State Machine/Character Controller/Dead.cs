using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : CharacterState
{
    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        StopPlayerMovement();
        EnableAgent(false);
        EnableRigidbody(true);
        currentCharacter.Animator.SetTrigger("Dead");
    }

    public override void Move(Vector2 input)
    {
    }

    public override void Salir()
    {
        base.Salir();
        currentCharacter.MovementComponent.enabled = true;
    }
}
