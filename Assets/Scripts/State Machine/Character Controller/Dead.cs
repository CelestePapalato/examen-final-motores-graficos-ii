using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : CharacterState
{
    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        StopPlayerActions();
        EnableAgent(false);
        EnableRigidbody(true);
        currentCharacter.MovementComponent.UpdatePositionON = false;
        currentCharacter.MovementComponent.UpdateRotationON = false;
    }

    public override void Salir()
    {
        base.Salir();
        currentCharacter.MovementComponent.enabled = true;
        currentCharacter.MovementComponent.UpdatePositionON = true;
        currentCharacter.MovementComponent.UpdateRotationON = true;
    }
}
