using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : CharacterState, IObjectTracker
{
    [Header("Chase configuration")]
    [SerializeField]
    private float pathUpdateRate;
    [SerializeField]
    private float distanceForNextState;
    [SerializeField]
    private float stateChangeCooldown;
    [SerializeField]
    [Tooltip("Si es true, se ignora nextState y se llama a Attack")]
    private bool attack = false;
    [SerializeField]
    private CharacterState nextState;

    public Transform Target { get; set; }

    private bool canChangeState = true;

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if(!currentCharacter || !currentCharacter.Agent || !Target)
        {
            //personaje.CambiarEstado(null);
            return;
        }
        if (currentCharacter.MovementComponent)
        {
            currentCharacter.MovementComponent.RigidBody.isKinematic = true;
            currentCharacter.MovementComponent.enabled = false;
        }
        currentCharacter.Agent.enabled = true;
        currentCharacter.Agent.speed = maxSpeed;

        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateRate);
    }

    public override void Salir()
    {
        base.Salir();
        currentCharacter.Agent.enabled = false;

        if (currentCharacter.MovementComponent)
        {
            currentCharacter.MovementComponent.enabled = true;
            currentCharacter.MovementComponent.RigidBody.isKinematic = false;
        }
    }

    private void OnEnable()
    {
        if(isActive && Target)
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
        float distance = Vector3.Distance(currentCharacter.Agent.transform.position, Target.position);
        if (distance > distanceForNextState || !canChangeState) { return; }
        if (attack)
        {
            currentCharacter?.Attack();
        }
        else if(nextState)
        {
            currentCharacter?.CambiarEstado(nextState);
        }
        StateChangeCooldown();
    }

    private void UpdatePath()
    {
        if (Target && isActive)
        {
            currentCharacter.Agent.destination = Target.position;
        }
        else
        {
            CancelInvoke(nameof(UpdatePath));
        }
    }

    private void StateChangeCooldown()
    {
        canChangeState = false;
        Invoke(nameof(EnableStateChange), stateChangeCooldown);
    }

    private void EnableStateChange()
    {
        canChangeState = true;
    }
}
