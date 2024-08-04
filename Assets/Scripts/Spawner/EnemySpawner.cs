using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    }

    public void StartSpawning() { }

    public void EndSpawning() { }

    private Enemy GetEnemy()
    {

        return null;
    }
}
