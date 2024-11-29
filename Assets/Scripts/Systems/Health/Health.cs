using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable, IObservableVariable, IBuffable
{
    Dictionary<IDamageDealer, List<float>> DamageDealerMemory = new Dictionary<IDamageDealer, List<float>>();

    [SerializeField] int maxHealth;
    [SerializeField] 
    [Tooltip("Realtime")] float damageMemoryTimeAlive = 2f;
    [SerializeField] bool enableInvincibilitySystem = false;
    [SerializeField] float invincibilityTime = .8f;
    public UnityAction<int, int> OnHealthUpdate;
    public UnityAction OnDead;
    public UnityAction<int, int> OnDamaged;
    public UnityAction<int, int> OnHealed;

    public event Action<int, int> OnUpdate;

    int health;
    bool invincibility = false;
    Collider col;
    Rigidbody rb;

    public int Current { get => health; }
    public int Max { get => maxHealth; }

    private void Awake()
    {
        health = maxHealth;
        col = GetComponent<Collider>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Start()
    {
        OnHealthUpdate?.Invoke(health, maxHealth);
        OnUpdate?.Invoke(health, maxHealth);
    }

    public void Accept(IBuff buff)
    {
        if (buff != null)
        {
            buff.Buff(this);
        }
    }

    public void Heal(int healPoints)
    {
        health = Mathf.Clamp(health + healPoints, 0, maxHealth);
        OnHealthUpdate?.Invoke(health, maxHealth);
        OnUpdate?.Invoke(health, maxHealth);
        OnHealed?.Invoke(health, maxHealth);
    }

    public void Damage(IDamageDealer damageDealer)
    {
        if (invincibility)
        {
            return;
        }
        if (DamageDealerMemory.ContainsKey(damageDealer))
        {
            if (DamageDealerMemory[damageDealer].Contains(damageDealer.ID)) { return; }
        }
        else
        {
            DamageDealerMemory.Add(damageDealer, new List<float>());
        }
        DamageDealerMemory[damageDealer].Add(damageDealer.ID);
        StartCoroutine(CleanDamageDealerMemory(damageDealer, damageDealer.ID));
        health = Mathf.Clamp(health - damageDealer.DamagePoints, 0, maxHealth);
        OnHealthUpdate?.Invoke(health, maxHealth);
        OnUpdate?.Invoke(health, maxHealth);
        OnDamaged?.Invoke(health, maxHealth);

        if (health <= 0 && OnDead != null)
        {
            col.enabled = false;
            OnDead();
            return;
        }
        if (enableInvincibilitySystem)
        {
            StartCoroutine(invincibilityEnabler());
            return;
        }
    }

    IEnumerator CleanDamageDealerMemory(IDamageDealer key, float id)
    {
        if (DamageDealerMemory.ContainsKey(key))
        {
            yield return new WaitForSecondsRealtime(damageMemoryTimeAlive);

            DamageDealerMemory[key].Remove(id);

            if (DamageDealerMemory[key].Count == 0) { DamageDealerMemory.Remove(key); }
        }
    }

    public void Hit(IDamageDealer damageDealer)
    {
        Vector3 position = transform.position;
        Vector3 impulseVector = position - damageDealer.Position;
        impulseVector.Normalize();
        if (rb)
        {
            rb.AddForce(impulseVector * damageDealer.Impulse, ForceMode.Impulse);
        }
    }

    IEnumerator invincibilityEnabler()
    {
        invincibility = true;
        col.enabled = false;
        yield return new WaitForSeconds(invincibilityTime);
        invincibility = false;
        col.enabled = true;
    }

    public void UpdateInvincibility(bool value)
    {
        StopCoroutine(invincibilityEnabler());
        invincibility = value;
        col.enabled = !value;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
