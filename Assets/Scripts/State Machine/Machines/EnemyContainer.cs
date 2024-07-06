using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyContainer : MonoBehaviour
{
    [SerializeField]
    int puntos;

    public UnityAction<EnemyContainer> OnDead;

    List<Enemy> partesVivas = new List<Enemy>();

    private void Awake()
    {
        Enemy[] partes = GetComponentsInChildren<Enemy>();
        foreach(Enemy enemigo in partes)
        {
            enemigo.OnDead += PartDestroyed;
        }
        partesVivas = partes.ToList();
    }

    private void PartDestroyed(Enemy part)
    {
        partesVivas.Remove(part);
        if(partesVivas.Count == 0)
        {
            Destroy(gameObject);
            //GameManager.instance.AddPoints(puntos);
            OnDead?.Invoke(this);
        }
    }
}
