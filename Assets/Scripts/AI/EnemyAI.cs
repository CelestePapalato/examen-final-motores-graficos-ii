using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private enum STATE { PATROL, CHASE, DEAD };
    private STATE state = STATE.PATROL;

    public static event Action<int> OnEnemyDead;

    private static Dictionary<Health, List<EnemyAI>> enemiesAttacking = new Dictionary<Health, List<EnemyAI>>();

    [Header("Enemy")]

    [SerializeField] int points;
    [SerializeField] float attackDistance;
    [SerializeField] float attackCooldown;

    [Header("Target Selection. Total Probability, 100% = 2")]
    [SerializeField]
    [Tooltip("1 = Low Health + High Health")]
    [Range(0f, 1f)] float lowHealthProbability;
    [SerializeField]
    [Tooltip("1 = Nearest + Furthest")]
    [Range(0f, 1f)] float nearestProbability;

    Character character;

    ItemSpawner itemSpawner;

    TargetDetection targetDetection;
    List<Health> enemiesDetected = new List<Health>();
    Health currentTarget;

    bool isInBattle = false;

    bool canAttack = true;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemSpawner = GetComponent<ItemSpawner>();
        targetDetection = transform.parent.GetComponent<TargetDetection>();
    }

    private void Start()
    {
        if (CheckTargetsInRange()) { return; }
        character?.StartPatrol();
    }

    private void OnEnable()
    {
        if (currentTarget)
        {
            currentTarget.OnDead += PlayerKilled;
        }
        else
        {
            GetPlayer();
        }
        if (targetDetection)
        {
            targetDetection.TargetFound.AddListener(TargetFound);
            targetDetection.TargetLost.AddListener(TargetLost);
        }
        if (character)
        {
            character.OnDead += Dead;
        }
    }

    private void OnDisable()
    {
        if (currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
        }
        if (targetDetection)
        {
            targetDetection.TargetFound?.RemoveListener(TargetFound);
            targetDetection.TargetLost?.RemoveListener(TargetLost);
        }
        if (character)
        {
            character.OnDead -= Dead;
        }
    }

    private void Update()
    {
        AttackTarget();
    }

    // # ---- CHASE STATE

    private void AttackTarget()
    {
        if (state != STATE.CHASE || !currentTarget || !canAttack) { return; }
        float distance = Vector3.Distance(currentTarget.transform.position, character.MovementComponent.transform.position);
        if(distance <= attackDistance)
        {
            character.Attack();
            canAttack = false;
            Invoke(nameof(EnableAttack), attackCooldown);
        }
    }

    private void EnableAttack()
    {
        canAttack = true;
    }

    // # ---- Target locator

    private bool CheckTargetsInRange() 
    {
        Transform[] targets = targetDetection.Targets;
        foreach (Transform t in targets)
        {
            TargetFound(t);
        }
        return targets.Length >= 1;
    }

    private void TargetFound(Transform target)
    {
       enemiesDetected.Add(target.GetComponentInChildren<Health>());
        if (!currentTarget)
        {
            TargetUpdate();
        }
    }

    private void TargetLost(Transform target)
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
            isInBattle = false;
            character.StartPatrol();
            return;
        }
        int r = UnityEngine.Random.Range(0, aliveTargets.Length);
        currentTarget = aliveTargets[r];
        currentTarget.OnDead += PlayerKilled;
        character.MoveTowards(currentTarget.transform);
        state = STATE.CHASE;
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
    
    private void Dead()
    {
        itemSpawner?.DropItem();
        OnEnemyDead?.Invoke(points);
        Destroy(gameObject, 5);
    }

}