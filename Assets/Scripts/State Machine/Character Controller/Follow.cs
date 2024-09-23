using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : CharacterState, IObjectTracker
{
    [Header("Path configuration")]
    [SerializeField]
    private float pathUpdateRate;
    public Transform Target { get; set; }

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
}
