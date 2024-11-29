using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power Up Data", menuName = "ScriptableObjects/Power Up Data", order = 2)]

public class PowerUp : ScriptableObject, IBuff
{
    [SerializeField] float _buffTime;
    [Header("Healing")]
    [SerializeField] int _healPoints;
    [Header("Weapon")]
    [SerializeField] float _fireRateMultiplier;
    [SerializeField] float _damageMultiplier;
    [Header("Movement")]
    [SerializeField] float _speedMultiplier;

    public void Buff(object o)
    {
        Health healthComponent = o as Health;
        if (healthComponent)
        {
            healthComponent.Heal(_healPoints);
        }
        Movement movementComponent = o as Movement;
        if (movementComponent)
        {
            movementComponent.SpeedPowerUp(_speedMultiplier, _buffTime);
        }
        Damage damageComponent = o as Damage;
        if (damageComponent)
        {
            damageComponent.DamagePowerUp(_damageMultiplier, _buffTime);
        }
        ProjectileShooter shooter = o as ProjectileShooter;
        if(shooter)
        {
            shooter.DamagePowerUp(_damageMultiplier, _buffTime);
        }
    }
}
