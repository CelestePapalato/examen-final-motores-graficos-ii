using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public static event Action<int> OnEnemyDead;
    public UnityAction<Enemy> OnDead;

    [SerializeField] int points;

    [Header("Target Selection. Total Probability, 100% = 2")]
    [SerializeField]
    [Tooltip("1 = Low Health + High Health")]
    [Range(0f, 1f)] float lowHealthProbability;
    [SerializeField]
    [Tooltip("1 = Nearest + Furthest")]
    [Range(0f, 1f)] float nearestProbability;

    Character chara;
    ItemSpawner itemSpawner;

    List<Health> enemiesDetected = new List<Health>();
    Health currentTarget;

    bool isInBattle = false;

    private void Awake()
    {
        chara = GetComponentInChildren<Character>();
        if (chara)
        {
            chara.OnDead += Dead;
        }
        itemSpawner = GetComponent<ItemSpawner>();
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
    }

    private void OnDisable()
    {
        if (currentTarget)
        {
            currentTarget.OnDead -= PlayerKilled;
        }
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
            chara.TargetUpdate(null); 
            isInBattle = false;
            return;
        }
        int r = UnityEngine.Random.Range(0, aliveTargets.Length);
        currentTarget = aliveTargets[r];
        currentTarget.OnDead += PlayerKilled;
        chara.TargetUpdate(currentTarget.transform);
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
    private void Dead()
    {
        //itemSpawner?.DropItem();
        OnEnemyDead?.Invoke(points);
    }


    private void OnDestroy()
    {
        OnDead?.Invoke(this);
        StopAllCoroutines();
    }
}