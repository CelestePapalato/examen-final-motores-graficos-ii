using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follow : CharacterState, IObjectTracker
{
    [Header("Path configuration")]
    [SerializeField]
    private float pathUpdateRate;
    [Header("Avoidance configuration")]
    [SerializeField]
    private float distance;
    [SerializeField]
    private float speedMultiplier;
    [SerializeField]
    private Vector2 runCompletelyAwayDistance;

    public Transform Target { get; set; }

    List<Transform> toAvoid = new List<Transform>();

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
        CancelInvoke(nameof(UpdatePath));
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

        Vector3 avoidVector = CalculateAvoidVector();
        currentCharacter.Agent.velocity = Vector3.Lerp(
            currentCharacter.Agent.desiredVelocity,
            avoidVector.normalized * currentCharacter.Agent.speed * speedMultiplier,
            Mathf.Clamp01(runCompletelyAwayDistance.x - avoidVector.magnitude / runCompletelyAwayDistance.y)
            );
    }

    private Vector3 CalculateAvoidVector()
    {
        Vector3 avoidVector = Vector3.zero;
        if(toAvoid.Count == 0) { return avoidVector; }
        Vector3 characterPosition = currentCharacter.MovementComponent.transform.position;
        Transform[] inRange = toAvoid.Where(x => Vector3.Distance(characterPosition, x.position) <= distance).ToArray();
        for (int i = 0; i < inRange.Length; i++)
        {
            Vector3 distance = inRange[i].position - characterPosition;
            if(distance.magnitude < avoidVector.magnitude || i == 0)
            {
                avoidVector = distance;
            }
        }
        return -avoidVector;
    }

    private void UpdatePath()
    {
        if (!Target || !isActive)
        {
            CancelInvoke(nameof(UpdatePath));
            return;
        }
        currentCharacter.Agent.SetDestination(Target.position);
    }

    public void AvoidTransform(Transform t)
    {
        if(toAvoid.Contains(t)) { return; }
        toAvoid.Add(t);
    }

    public void StopAvoiding(Transform t)
    {
        if (toAvoid.Contains(t)) { toAvoid.Remove(t); }
    }
}
