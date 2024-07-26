using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable, IHittable
{
    [SerializeField] int maxHealth;
    [SerializeField] float invincibilityTime;
    public UnityAction<int, int> OnHealthUpdate;
    public UnityAction OnDead;
    public UnityAction<int, int> OnDamaged;
    public UnityAction<int, int> OnHealed;

    int health;
    bool invincibility = false;
    Collider col;
    Rigidbody rb;

    public int CurrentHealth { get => health; }
    public int MaxHealth { get => maxHealth; }

    private void Awake()
    {
        health = maxHealth;
        col = GetComponent<Collider>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Start()
    {
        OnHealthUpdate?.Invoke(health, maxHealth);
    }

    public void Heal(int healPoints)
    {
        health = Mathf.Clamp(health + healPoints, 0, maxHealth);
        OnHealthUpdate?.Invoke(health, maxHealth);
        OnHealed?.Invoke(health, maxHealth);
    }

    public void Damage(IDamageDealer damageDealer)
    {
        if (invincibility)
        {
            return;
        }
        health = Mathf.Clamp(health - damageDealer.DamagePoints, 0, maxHealth);
        OnHealthUpdate?.Invoke(health, maxHealth);
        OnDamaged?.Invoke(health, maxHealth);

        if (health <= 0 && OnDead != null)
        {
            col.enabled = false;
            OnDead();
            return;
        }
        StartCoroutine(invincibilityEnabler());
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
