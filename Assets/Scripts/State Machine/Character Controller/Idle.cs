using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Idle : CharacterState
{
    public override void Salir()
    {
        base.Salir();
        //currentCharacter.MovementComponent.Direction = Vector2.zero;
    }

    public override void Attack() { }

    public override void Evade() { }

}
