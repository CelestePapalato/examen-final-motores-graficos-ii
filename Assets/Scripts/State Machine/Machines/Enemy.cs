using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public static event Action<int> OnEnemyDead;

    [Header("Enemy")]

    [SerializeField] int points;
    [SerializeField] float attackDistance;

    [Header("Target Selection. Total Probability, 100% = 2")]
    [SerializeField]
    [Tooltip("1 = Low Health + High Health")]
    [Range(0f, 1f)] float lowHealthProbability;
    [SerializeField]
    [Tooltip("1 = Nearest + Furthest")]
    [Range(0f, 1f)] float nearestProbability;

    ItemSpawner itemSpawner;

    List<Health> enemiesDetected = new List<Health>();
    Health currentTarget;

    bool isInBattle = false;

    bool canAttack = true;

    protected override void Awake()
    {
        base.Awake();
        itemSpawner = GetComponent<ItemSpawner>();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        if (currentTarget)
        {
            currentTarget.OnDead += PlayerKilled;
        }
        else
        {
            GetPlayer();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!isInBattle || !currentTarget) { return; }
        float distanceToTarget = Vector3.Distance(MovementComponent.transform.position, currentTarget.transform.position);
        canAttack = (distanceToTarget <= attackDistance);       
    }

    public void TargetFound(Transform target)
    {
       enemiesDetected.Add(target.GetComponentInChildren<Health>());
        if (!currentTarget)
        {
            TargetUpdate();
        }
    }

    public void TargetLost(Transform target)
    {
        Health character = target.GetComponentInChildren<Health>();
        if (!enemiesDetected.Contains(character)) { Debug.Log("papas"); return; }
        enemiesDetected.Remove(character);
        if (character == currentTarget)
        {
            TargetUpdate();
        }
    }

    private void TargetUpdate()
    {
        Health[] aliveTargets = enemiesDetected.Where(x => x.CurrentHealth > 0).ToArray();
        if (enemiesDetected.Count == 0 || aliveTargets.Length == 0)
        {
            currentTarget = null; 
            TrackerUpdate(null); 
            isInBattle = false;
            return;
        }
        int r = UnityEngine.Random.Range(0, aliveTargets.Length);
        currentTarget = aliveTargets[r];
        currentTarget.OnDead += PlayerKilled;
        TrackerUpdate(currentTarget.transform);
        isInBattle = true;
    }

    private void GetPlayer()
    {
        Player aux = Player.RandomAlivePlayer;
        if (!aux) { return; }
       // currentTarget = aux.PlayerCharacter;
        //currentTarget.OnDead += PlayerKilled;
        //chara.TargetUpdate(currentTarget.transform);
    }

    private void PlayerKilled()
    {
        if (currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
        }
        TargetUpdate();
    }
    
    protected override void Dead()
    {
        base.Dead();
        itemSpawner?.DropItem();
        OnEnemyDead?.Invoke(points);
        Destroy(gameObject);
    }

    public override void Attack()
    {
        if(isInBattle && !canAttack) { return; }
        base.Attack();
    }
}