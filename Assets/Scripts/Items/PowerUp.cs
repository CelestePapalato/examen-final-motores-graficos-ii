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
        CharacterState player = o as CharacterState;
        if (player)
        {
            //player.SpeedPowerUp(_speedMultiplier, _buffTime);
            //player.DamagePowerUp(_damageMultiplier, _buffTime);
        }
        /*
        Weapon weapon = o as Weapon;
        if (weapon)
        {
            weapon.FireRateBonus(_fireRateMultiplier, _buffTime);
            weapon.DamageBonus(_damageMultiplier, _buffTime);
        }
        */
    }
}
