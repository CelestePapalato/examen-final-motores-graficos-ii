using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable, IHittable
{
    [SerializeField] int maxHealth;
    [SerializeField] float invincibilityTime;
    public UnityAction<int, int> onHealthUpdate;
    public UnityAction onNoHealth;
    public UnityAction<int, int> onDamaged;
    public UnityAction<int, int> onHealed;

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
        onHealthUpdate?.Invoke(health, maxHealth);
    }

    public void Heal(int healPoints)
    {
        health = Mathf.Clamp(health + healPoints, 0, maxHealth);
        onHealthUpdate?.Invoke(health, maxHealth);
        onHealed?.Invoke(health, maxHealth);
    }

    public void Damage(IDamageDealer damageDealer)
    {
        if (invincibility)
        {
            return;
        }
        health = Mathf.Clamp(health - damageDealer.DamagePoints, 0, maxHealth);
        onHealthUpdate?.Invoke(health, maxHealth);
        onDamaged?.Invoke(health, maxHealth);

        if (health <= 0 && onNoHealth != null)
        {
            col.enabled = false;
            onNoHealth();
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
