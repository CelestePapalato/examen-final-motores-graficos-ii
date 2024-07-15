using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator UpdateNavMeshTarget()
    {
        while(isActive)
        {
            yield return new WaitForSeconds(pathUpdateRate);
            if(agent.enabled && Target != null)
            {
                agent.destination = Target.position;
            }
        }
    }
}
