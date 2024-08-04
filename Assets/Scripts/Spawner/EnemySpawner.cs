using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemySpawnData
{
    public Enemy enemy;
    public float spawnCooldown;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float startWaitTime;
    [SerializeField] float spawnCooldown;
    [SerializeField] EnemySpawnData[] spawnData;
    [SerializeField] Transform spawnTransform;

    public UnityEvent Start;
    public UnityEvent End;

    Vector3 spawnPosition;

    List<Enemy> alreadySpawned = new List<Enemy>();

    private void Awake()
    {
        if (!spawnTransform)
        {
            spawnPosition = transform.position;
        }
        else
        {
            spawnPosition = spawnTransform.position;
        }
        Invoke(nameof(StartSpawning), startWaitTime);
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        CancelInvoke(nameof(StartSpawning));
        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), spawnCooldown, spawnCooldown);
        Start?.Invoke();
    }

    [ContextMenu("End Spawning")]
    public void EndSpawning()
    {
        CancelInvoke(nameof(StartSpawning));
        CancelInvoke(nameof(Spawn));
        End?.Invoke();
    }

    private Enemy GetEnemy()
    {

        return null;
    }

    private void Spawn()
    {

    }
}
