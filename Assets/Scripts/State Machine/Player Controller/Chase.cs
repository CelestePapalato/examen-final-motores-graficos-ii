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

        StartCoroutine(nameof(UpdatePath));
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
            StartCoroutine(nameof(UpdatePath));
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines(); 
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

    IEnumerator UpdatePath()
    {
        while (Target && isActive)
        {
            currentCharacter.Agent.destination = Target.position;
            yield return new WaitForSeconds(pathUpdateRate);
        }
    }
}
