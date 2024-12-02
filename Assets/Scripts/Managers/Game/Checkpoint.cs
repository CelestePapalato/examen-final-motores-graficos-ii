using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{

    private static Checkpoint lastCheckPoint = null;

    public static Action OnRespawnFailed;
    public static Action<Transform[]> OnRespawnSucceeded;

    [SerializeField]
    Transform[] spawnPoints;

    public Transform[] SpawnPoints { get => (Transform[]) spawnPoints.Clone(); } 

    private void Start()
    {
        if (spawnPoints.Length == 0 || spawnPoints == null)
        {
            spawnPoints = new Transform[] { transform };
        }
    }

    public void NewCheckpointUnlocked()
    {
        lastCheckPoint = this;
        Debug.Log("New Checkpoint");
    }

    public static bool TryRespawning()
    {
        if (!lastCheckPoint)
        {
            OnRespawnFailed?.Invoke();
            return false;
        }

        OnRespawnSucceeded?.Invoke(lastCheckPoint.SpawnPoints);
        return true;
    }
}
