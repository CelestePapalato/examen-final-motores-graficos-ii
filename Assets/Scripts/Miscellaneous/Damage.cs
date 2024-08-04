using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour, IDamageDealer, IBuffable
{
    [SerializeField] int damagePoints;
    [SerializeField] float impulse;

    public UnityAction DamageDealed;

    public Character Owner;

    public int DamagePoints { get { return (int) (damagePoints * damageMultiplier); } }

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }

    public float Impulse { get {  return impulse; } }
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

    private void OnTriggerEnter(Collider other)
    {
        bool damageDealed = false;
        IDamageable enemyDamageable = other.GetComponentInChildren<IDamageable>();
        IHittable enemyHittable = other.GetComponentInChildren<IHittable>();

        Debug.Log(Owner.name + enemyDamageable);

        Debug.Log(enemyHittable);
        Debug.Log(enemyDamageable);
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
        CancelInvoke(nameof(DamagePowerUpDisabler));
        Invoke(nameof(DamagePowerUpDisabler), time);
    }

    private void DamagePowerUpDisabler()
    {
        damageMultiplier = 1;
    }

}
