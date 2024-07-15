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

    [Header("Enemy")]

    [SerializeField] float distanceToAttack;
    [SerializeField] float attackCooldown;
    [SerializeField] int points;

    Health healthComponent;
    ItemSpawner itemSpawner;
    Animator animator;
    NavMeshAgent agent;
    Player player;

    IObjectTracker[] trackers;

    float originalSpeed;

    bool canAttack = true;

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

        if (player && canAttack)
        {
            float distance = Vector3.Distance(transform.position, player.CharacterTransform.position);
            if (distance <= distanceToAttack)
            {
                animator.SetTrigger("Attack");
                StartCoroutine(AttackCooldown());
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
            tracker.Target = player.CharacterTransform;
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

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
        StopAllCoroutines();
    }
}