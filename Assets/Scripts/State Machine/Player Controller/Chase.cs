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
    private CharacterState nextState;

    public Transform Target { get; set; }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if(!currentCharacter || !currentCharacter.Agent || !Target)
        {
            personaje.CambiarEstado(null);
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
        StopAllCoroutines();
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
        CancelInvoke(); 
    }

    public override void Actualizar()
    {
        base.Actualizar();
        float distance = Vector3.Distance(transform.position, Target.position);
        if (distance <= distanceForNextState)
        {
            currentCharacter.CambiarEstado(nextState);
        }
    }

    private void UpdatePath()
    {
        if (Target && isActive)
        {
            currentCharacter.Agent.destination = Target.position;
        }
        else
        {
            CancelInvoke();
        }
    }
}
