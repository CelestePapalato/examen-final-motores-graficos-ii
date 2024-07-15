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

    IObjectTracker[] trackers;

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

        trackers = GetComponentsInChildren<IObjectTracker>();
    }

    private void Start()
    {
        GetPlayer();
    }

    private void OnEnable()
    {
        if (player)
        {
            player.OnDead += PlayerKilled;
        }
    }

    private void OnDisable()
    {
        if (player)
        {
            player.OnDead -= PlayerKilled;
        }
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

    private void GetPlayer()
    {
        player = Player.RandomAlivePlayer;
        player.OnDead += PlayerKilled;
        foreach (var tracker in trackers)
        {
            tracker.Target = player.transform;
        }
    }

    private void PlayerKilled()
    {
        if (player)
        {
            player.OnDead -= PlayerKilled;
        }
        GetPlayer();
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