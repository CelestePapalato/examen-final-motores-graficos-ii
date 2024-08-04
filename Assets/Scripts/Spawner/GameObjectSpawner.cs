using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SpawnData
{
    public GameObject gameObject;
    public float spawnCooldown;
}

public class GameObjectSpawner : MonoBehaviour
{
    [SerializeField] float startWaitTime;
    [SerializeField] float spawnCooldown;
    [SerializeField] SpawnData[] spawnData;
    [SerializeField] Transform spawnTransform;

    public UnityEvent Start;
    public UnityEvent End;

    Vector3 spawnPosition;
    Quaternion spawnRotation;

    List<SpawnData> currentSpawnList;
    List<SpawnData> alreadySpawned = new List<SpawnData>();

    private void Awake()
    {
        currentSpawnList = spawnData.ToList();
        if (!spawnTransform)
        {
            spawnTransform = transform;
        }
        Invoke(nameof(StartSpawning), startWaitTime);
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        CancelInvoke(nameof(StartSpawning));
        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), 0, spawnCooldown);
        Start?.Invoke();
    }

    [ContextMenu("End Spawning")]
    public void EndSpawning()
    {
        CancelInvoke(nameof(StartSpawning));
        CancelInvoke(nameof(Spawn));
        End?.Invoke();
    }

    private SpawnData GetEnemy()
    {
        if (currentSpawnList.Count == 0) { return null; }
        int r = UnityEngine.Random.Range(0, currentSpawnList.Count);
        return currentSpawnList[r];
    }

    private void Spawn()
    {
        SpawnData toSpawn = GetEnemy();
        if(toSpawn == null) { return; }

        Instantiate(toSpawn.gameObject, spawnTransform);

        StartCoroutine(ControlSpawnList(toSpawn));
    }

    IEnumerator ControlSpawnList(SpawnData spawnData)
    {
        alreadySpawned.Add(spawnData);
        currentSpawnList.Remove(spawnData);
        yield return new WaitForSeconds(spawnData.spawnCooldown);
        alreadySpawned.Remove(spawnData);
        currentSpawnList.Add(spawnData);
    }

}
