using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicMusic : MonoBehaviour
{
    public UnityEvent OnPlayerInChase;
    public UnityEvent OnPlayerSafe;

    bool active = false;

    List<EnemyAI> enemiesChasingPlayer = new List<EnemyAI>();

    private void OnEnable()
    {
        EnemyAI.PlayerInRange += EnemyChasingPlayer;
        EnemyAI.PlayerOutOfRange += EnemyRetires;
    }

    private void OnDisable()
    {
        EnemyAI.PlayerInRange -= EnemyChasingPlayer;
        EnemyAI.PlayerOutOfRange -= EnemyRetires;
    }

    private void EnemyChasingPlayer(EnemyAI enemyAI)
    {
        if(!enemiesChasingPlayer.Contains(enemyAI))
        {
            enemiesChasingPlayer.Add(enemyAI);
        }
        if(!active)
        {
            active = true;
            OnPlayerInChase?.Invoke();
        }
    }

    private void EnemyRetires(EnemyAI enemyAI)
    {
        if (enemiesChasingPlayer.Contains(enemyAI))
        {
            enemiesChasingPlayer.Remove(enemyAI);
        }
        if(enemiesChasingPlayer.Count == 0)
        {
            active = false;
            OnPlayerSafe?.Invoke();
        }
    }

}
