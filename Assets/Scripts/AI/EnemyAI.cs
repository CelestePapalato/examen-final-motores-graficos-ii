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
    public static event Action<EnemyAI> PlayerInRange;
    public static event Action<EnemyAI> PlayerOutOfRange;
    bool playerInRange = false;

    private static Dictionary<Health, List<EnemyAI>> enemiesAttacking = new Dictionary<Health, List<EnemyAI>>();

    [Header("Enemy")]

    [SerializeField] int points;
    [SerializeField] float attackDistance;
    [SerializeField] float attackCooldown;

    [Header("Target Selection")]
    [SerializeField]
    [Range(0, 10)] int changeTargetProbability;
    [SerializeField]
    [Range(0, 10)] int lowestHealthProbability;

    Character character;

    ItemSpawner itemSpawner;

    TargetDetection targetDetection;
    List<Health> enemiesDetected = new List<Health>();
    Health currentTarget;

    bool isInBattle = false;
    bool isDead = false;
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
        if (state != STATE.CHASE || !currentTarget || !canAttack || isDead) { return; }
        float distance = Vector3.Distance(currentTarget.transform.position, character.MovementComponent.transform.position);
        if(distance <= attackDistance)
        {
            character.Attack();
            canAttack = false;
            character.Avoid(currentTarget.transform);
            Invoke(nameof(EnableAttack), attackCooldown);
        }
    }

    private void EnableAttack()
    {
        canAttack = true;
        if (currentTarget)
        {
            character.StopAvoiding(currentTarget.transform);
        }
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
       enemiesDetected.RemoveAll(x => !x);
       enemiesDetected.Add(target.GetComponentInChildren<Health>());
        if (!currentTarget || ProbabilityCheck(changeTargetProbability))
        {
            Debug.Log("Change target");
            TargetUpdate();
        }
        if (target.CompareTag("Player") && !playerInRange)
        {
            PlayerInRange?.Invoke(this);
            playerInRange = true;
        }
    }

    private void TargetLost(Transform target)
    {
        enemiesDetected.RemoveAll(x => !x);
        Health targetCharacter = target.GetComponentInChildren<Health>();
        if (!enemiesDetected.Contains(targetCharacter)) { return; }
        enemiesDetected.Remove(targetCharacter);
        if (targetCharacter == currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
            character.StopAvoiding(currentTarget.transform);
            TargetUpdate();
        }
        Health obj = enemiesDetected.FirstOrDefault(x => x.CompareTag("Player"));
        if (obj == null)
        {
            PlayerOutOfRange?.Invoke(this);
            playerInRange = false;
        }
    }

    private void TargetUpdate()
    {
        if (isDead) { return; }
        if (currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
            character.StopAvoiding(currentTarget.transform);
        }
        Health[] aliveTargets = enemiesDetected.Where(x => x.Current > 0).ToArray();
        if (enemiesDetected.Count == 0 || aliveTargets.Length == 0)
        {
            currentTarget = null; 
            isInBattle = false;
            character.StartPatrol();
            return;
        }
        if (ProbabilityCheck(lowestHealthProbability))
        {
            Debug.Log("Pick enemy with lowest health");
            currentTarget = MinorHealthEnemy();
        }
        else
        {
            int r = UnityEngine.Random.Range(0, aliveTargets.Length);
            Debug.Log(r);
            currentTarget = aliveTargets[r];
        }
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
            character.StopAvoiding(currentTarget.transform);
        }
        TargetUpdate();
    }
    
    private void Dead()
    {
        isDead = true;
        itemSpawner?.DropItem();
        OnEnemyDead?.Invoke(points);
        Destroy(gameObject, 5);
        PlayerOutOfRange?.Invoke(this);
        playerInRange = false;
        this.enabled = false;
    }

    private bool ProbabilityCheck(float probability)
    {
        int r = UnityEngine.Random.Range(0, 10);
        Debug.Log("probability check: " + probability + ", " + r);
        return r <= probability;
    }

    private Health MinorHealthEnemy()
    {
        Health newTarget = null;
        foreach(Health health in enemiesDetected)
        {
            if(newTarget == null || health.Current < newTarget.Current)
            {
                newTarget = health;
            }
        }
        return newTarget;
    }
}