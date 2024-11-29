using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour, IDamageDealer, IBuffable
{
    [SerializeField] int damagePoints;
    [SerializeField] float impulse;
    [Header("Debug")]
    [SerializeField] int currentDamagePoints;

    public UnityAction DamageDealed;

    public Character Owner;

    public int DamagePoints { get { return (int)(damagePoints * damageMultiplier); } set => damagePoints = (value > 0) ? value : damagePoints; }

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }

    public float Impulse { get {  return impulse; } }

    private float id;
    public float ID {  get => id; }

    public Vector3 Position
    {
        get
        {
            if (Owner)
            {
                return Owner.MovementComponent.transform.position;
            }
            else
            {
                return transform.position;
            }
        }
    }

    Collider col;

    bool updateID = true;

    private void Awake()
    {
        col = GetComponent<Collider>();

        currentDamagePoints = damagePoints;
    }

    private void FixedUpdate()
    {
        if (!col.enabled && !updateID)
        {
            updateID = true;
        }

        if(col.enabled && updateID)
        {
            updateID = false;
            id = Time.timeSinceLevelLoad;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool damageDealed = false;
        IDamageable enemyDamageable = other.GetComponentInChildren<IDamageable>();
        IHittable enemyHittable = other.GetComponentInChildren<IHittable>();

        if (enemyDamageable != null)
        {
            enemyDamageable.Damage(this);
            damageDealed = true;
        }
        if (enemyHittable != null)
        {
            enemyHittable.Hit(this);
            damageDealed = true;
        }
        if (damageDealed && DamageDealed != null)
        {
            DamageDealed();
        }
    }

    public void Accept(IBuff buff)
    {
        if (buff == null) return;
        buff.Buff(this);
    }

    public void DamagePowerUp(float multiplier, float time)
    {
        multiplier = Mathf.Max(multiplier, 1f);
        if (multiplier == 1)
        {
            return;
        }
        damageMultiplier = multiplier;
        float desiredDamage = damageMultiplier * damagePoints;
        float desiredDamagePoints = Mathf.Ceil(desiredDamage);
        damageMultiplier = desiredDamagePoints / damagePoints;
        currentDamagePoints = (int) (damageMultiplier * damagePoints);
        CancelInvoke(nameof(DamagePowerUpDisabler));
        Invoke(nameof(DamagePowerUpDisabler), time);
    }

    private void DamagePowerUpDisabler()
    {
        damageMultiplier = 1;
        currentDamagePoints = damagePoints;
    }

}
