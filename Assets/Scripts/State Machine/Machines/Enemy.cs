using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : StateMachine
{
    public static event Action<int> EnemyDead;
    public UnityAction<Enemy> OnDead;

    [SerializeField] float distanceToAttack;
    [SerializeField] int points;

    Health healthComponent;
    ItemSpawner itemSpawner;
    Animator animator;
    NavMeshAgent agent;
    Player player;

    float originalSpeed;

    protected override void Awake()
    {
        base.Awake();
        healthComponent = GetComponentInChildren<Health>();
        if (healthComponent)
        {
            healthComponent.onNoHealth += Dead;
            healthComponent.onDamaged += DamageReceived;
        }
        itemSpawner = GetComponent<ItemSpawner>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
    }

    protected void Start()
    {
        player = Player.RandomAlivePlayer;
    }

    protected override void Update()
    {
        agent.enabled = animator.GetBool("CanMove");

        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= distanceToAttack)
            {
                animator.SetTrigger("Attack");
            }
        }

        float speedBlend = agent.speed / originalSpeed;
        animator?.SetFloat("Speed", speedBlend);
        base.Update();
    }

    private void Dead()
    {
        itemSpawner?.DropItem();
        Destroy(gameObject);
        EnemyDead.Invoke(points);
    }

    private void DamageReceived(int health, int maxHealth)
    {
        //DamageReceived();
    }

    private void OnDestroy()
    {
        OnDead?.Invoke(this);
    }
}