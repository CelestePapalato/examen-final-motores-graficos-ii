using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static List<Player> currentPlayers = new List<Player>();
    public static Player[] CurrentPlayers { get { return currentPlayers.ToArray(); } }

    public static Player RandomPlayer
    {
        get
        {
            int r = UnityEngine.Random.Range(0, currentPlayers.Count);
            return currentPlayers[r];
        }
    }

    public static Player RandomAlivePlayer
    {
        get
        {
            if (currentPlayers.Count == 0) { return null; }

            Player[] alivePlayers = currentPlayers.Where(x => !x.IsDead).ToArray();
            int r = UnityEngine.Random.Range(0, alivePlayers.Length);
            return alivePlayers[r];
        }
    }

    public static event Action<Player> OnPlayerDead;
    public Transform CharacterTransform { get => (chara) ? chara.MovementComponent.transform : transform; }

    public Character PlayerCharacter { get => chara; }


    private bool _isDead = false;
    public bool IsDead { get => _isDead; private set => _isDead = value; }

    Character chara;
    Healthbar healthBar;

    private void Start()
    {
        UpdateHealthbar();
    }

    private void OnEnable()
    {
        currentPlayers.Add(this);
        GetHealthbar();
        if (GetCharacter())
        {
            chara.OnDead += Dead;
        }
    }

    private void OnDisable()
    {
        currentPlayers.Remove(this);
        if (GetCharacter())
        {
            chara.OnDead -= Dead;
        }
    }

    private bool GetCharacter()
    {
        if (!chara)
        {
            chara = GetComponentInChildren<Character>();
        }
        UpdateHealthbar();
        return chara;
    }

    private bool GetHealthbar()
    {
        if (!healthBar)
        {
            healthBar = GetComponentInChildren<Healthbar>();
        }
        return healthBar;
    }

    private void UpdateHealthbar()
    {
        Health health = chara.HealthComponent;
        if (healthBar && health)
        {
            healthBar.HealthComponent = health;
        }
    }

    private void Dead()
    {
        this.enabled = false;
        _isDead = true;
        OnPlayerDead?.Invoke(this);
    }
}
