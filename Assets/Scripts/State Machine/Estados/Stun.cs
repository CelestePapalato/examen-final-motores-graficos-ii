using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stun : MonoBehaviour
{
    [SerializeField] int hitsNeeded;
    [SerializeField] float stunLength;
    [SerializeField]
    [Range(0f, 1f)] float speedModifier;

    NavMeshAgent agent;
    Health health;

    float originalSpeed;

    int hitsReceived = 0;

    bool stunApplied = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        health = GetComponentInChildren<Health>();
    }

    private void StunStart(int health, int maxHealth)
    {
        if(hitsNeeded == 0)
        {
            Debug.LogWarning("No se puede dividir por cero. HitsNeeded no puede valer cero");
            return;
        }
        hitsReceived++;
        if ( hitsReceived % hitsNeeded != 0)
        {
            return;
        }
        if (stunApplied)
        {
            CancelInvoke(nameof(StunEnd));
            StunEnd();
        }
        hitsReceived = 0;
        agent.speed *= speedModifier;
        stunApplied = true;
        Invoke(nameof(StunEnd), stunLength);
    }

    private void StunEnd()
    {
        agent.speed /= speedModifier;
        stunApplied = false;
    }

    private void OnEnable()
    {
        if (health)
        {
            health.onDamaged += StunStart;
        }
    }

    private void OnDisable()
    {
        if (health)
        {
            health.onDamaged -= StunStart;
        }
    }
}
