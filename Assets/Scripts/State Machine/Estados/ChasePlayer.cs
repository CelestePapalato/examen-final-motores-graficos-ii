using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChasePlayer : State, IObjectTracker
{
    [SerializeField] float pathUpdateRate;
    NavMeshAgent agent;
    Rigidbody rb;

    public Transform Target { get; set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if (rb)
        {
            rb.isKinematic = true;
        }
        if (!agent) { agent = GetComponent<NavMeshAgent>(); }
        agent.enabled = true;
        StartCoroutine(UpdateNavMeshTarget());
    }

    public override void Salir()
    {
        base.Salir();
        if (rb)
        {
            rb.isKinematic = false;
        }
        StopAllCoroutines();
    }

    IEnumerator UpdateNavMeshTarget()
    {
        while(isActive && Target)
        {
            yield return new WaitForSeconds(pathUpdateRate);
            if(agent.enabled)
            {
                agent.destination = Target.position;
            }
        }
    }
}
