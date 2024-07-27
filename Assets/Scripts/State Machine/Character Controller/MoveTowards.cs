using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : CharacterState
{
    [Header("Move Towards State Configuration")]
    [SerializeField] Transform destination;
    [SerializeField] float pathUpdateRate;
    [SerializeField] float tolerance;
    [SerializeField] State nextState;

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if (!currentCharacter || !currentCharacter.Agent || !destination)
        {
            //personaje.CambiarEstado(null);
            return;
        }
        StopPlayerMovement();
        EnableRigidbody(false);
        EnableAgent(true);
        currentCharacter.Agent.speed = maxSpeed;
        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateRate);
    }

    public override void Salir()
    {
        base.Salir();
        EnableAgent(false);
        EnableRigidbody(true);
        ResumePlayerMovement();
    }

    private void OnEnable()
    {
        if (isActive && destination)
        {
            InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateRate);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdatePath));
    }

    public override void Actualizar()
    {
        float speedFactor = currentCharacter.Agent.velocity.magnitude / maxSpeed;
        currentCharacter.Animator?.SetFloat("Speed", speedFactor);
        float distance = Vector3.Distance(transform.position, destination.position);
        if (distance > tolerance || !nextState) { return; }
        currentCharacter?.CambiarEstado(nextState);
    }

    private void UpdatePath()
    {
        if (destination && isActive)
        {
            currentCharacter.Agent.destination = destination.position;
        }
        else
        {
            CancelInvoke(nameof(UpdatePath));
        }
    }
}
