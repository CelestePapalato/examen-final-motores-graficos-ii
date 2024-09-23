using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Patrol : CharacterState
{
    [Header("Patrol State Configuration")]
    [SerializeField] Transform[] destination;
    [SerializeField] float pathUpdateRate;
    [SerializeField] 
    [Tooltip("If looping is activated, Next State won't be called")]
                     bool loop;
    [SerializeField] bool restartOnExit;
    [SerializeField] float tolerance;
    [SerializeField] State nextState;

    Transform currentDestination;
    int currentDestinationIndex = -1;

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if (!currentCharacter || !currentCharacter.Agent)
        {
            //personaje.CambiarEstado(null);
            return;
        }
        StopPlayerMovement();
        currentCharacter.MovementComponent.enabled = false;
        EnableRigidbody(false);
        EnableAgent(true);
        currentCharacter.Agent.speed = maxSpeed;
        if(restartOnExit || currentDestinationIndex < 0) { currentDestinationIndex = -1; NextDestination(); }
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
        if (isActive && currentDestination)
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
        if (!currentDestination) { return; }
        float speedFactor = currentCharacter.Agent.velocity.magnitude / maxSpeed;
        currentCharacter.Animator?.SetFloat("Speed", speedFactor);
        float distance = Vector3.Distance(currentCharacter.Agent.transform.position, currentDestination.position);
        if (distance > tolerance || !(nextState || loop)) { return; }
        if (loop || currentDestinationIndex < destination.Length - 1)
        {
            NextDestination();
            return;
        }
        currentCharacter?.CambiarEstado(nextState);
    }

    private void NextDestination()
    {
        currentDestinationIndex++;
        if(currentDestinationIndex > destination.Length - 1) { currentDestinationIndex = 0; }
        currentDestination = destination[currentDestinationIndex];
    }

    private void UpdatePath()
    {
        if(destination.Length == 0) { return; }
        if (currentDestination && isActive)
        {
            currentCharacter.Agent.destination = currentDestination.position;
        }
        else
        {
            CancelInvoke(nameof(UpdatePath));
        }
    }
}
