using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : StateMachine
{
    [SerializeField] int points;
    public UnityAction<Enemy> OnDead;
    Health healthComponent;
    ItemSpawner itemSpawner;

    protected override void Awake()
    {
        base.Awake();
        healthComponent = GetComponentInChildren<Health>();
        if (healthComponent)
        {
            healthComponent.onNoHealth += Dead;
        }
        itemSpawner = GetComponent<ItemSpawner>();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void Dead()
    {
        itemSpawner?.DropItem();
        Destroy(gameObject);
        //GameManager.instance.AddPoints(points);
    }

    private void OnDestroy()
    {
        OnDead?.Invoke(this);
    }
}
