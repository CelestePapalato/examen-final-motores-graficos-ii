using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour, IBuffable
{
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    Projectile toInstance;

    public Character Owner;

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }

    public void Shoot()
    {
        Projectile instance = Instantiate(toInstance, spawnPoint.position, Quaternion.identity);
        Damage damage = instance.GetComponentInChildren<Damage>();
        if(damage != null)
        {
            damage.DamageMultiplier = damageMultiplier;
            damage.Owner = Owner;
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
