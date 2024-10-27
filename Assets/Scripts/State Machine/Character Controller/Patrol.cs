using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Patrol : CharacterState, IAvoidObject
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
    [Header("Avoidance configuration")]
    [SerializeField]
    private float distance;
    [SerializeField]
    private float speedMultiplier;
    [SerializeField]
    private Vector2 runCompletelyAwayDistance;

    List<Transform> toAvoid = new List<Transform>();

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
        StopPlayerActions();
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
        ApplyAvoidanceVector();

        float speedFactor = currentCharacter.Agent.velocity.magnitude / maxSpeed;
        currentCharacter.Animator?.SetFloat("Speed", speedFactor);

        if (!currentDestination) { return; }

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
            currentCharacter.Agent.SetDestination(currentDestination.position);
        }
        else
        {
            CancelInvoke(nameof(UpdatePath));
        }
    }
    private bool CalculateAvoidVector(out Vector3 output)
    {
        output = Vector3.zero;
        if (toAvoid.Count == 0) { return false; }
        Vector3 characterPosition = currentCharacter.MovementComponent.transform.position;
        Transform[] inRange = toAvoid.Where(x => Vector3.Distance(characterPosition, x.position) <= distance).ToArray();
        if (inRange.Length == 0) { return false; }
        for (int i = 0; i < inRange.Length; i++)
        {
            Vector3 distance = inRange[i].position - characterPosition;
            if (distance.magnitude < output.magnitude || i == 0)
            {
                output = distance;
            }
        }
        output *= -1;
        return true;
    }

    private void ApplyAvoidanceVector()
    {
        Vector3 avoidVector;
        if (!CalculateAvoidVector(out avoidVector)) { return; }
        currentCharacter.Agent.velocity = Vector3.Lerp(
            currentCharacter.Agent.desiredVelocity,
            avoidVector.normalized * currentCharacter.Agent.speed * speedMultiplier,
            Mathf.Clamp01(runCompletelyAwayDistance.x - avoidVector.magnitude / runCompletelyAwayDistance.y)
            );
    }

    public void AvoidTransform(Transform t)
    {
        if (toAvoid.Contains(t)) { return; }
        toAvoid.Add(t);
    }

    public void StopAvoiding(Transform t)
    {
        if (toAvoid.Contains(t)) { toAvoid.Remove(t); }
    }
}
