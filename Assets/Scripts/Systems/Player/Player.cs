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
        chara = GetComponentInChildren<Character>();
        healthBar = GetComponentInChildren<Healthbar>();
        if (healthBar && chara)
        {
            healthBar.HealthComponent = chara.HealthComponent;
        }
    }

    private void OnEnable()
    {
        currentPlayers.Add(this);
        if (chara)
        {
            chara.OnDead += Dead;
        }
    }

    private void OnDisable()
    {
        currentPlayers.Remove(this);
        if (chara)
        {
            chara.OnDead -= Dead;
        }
    }

    private void Dead()
    {
        this.enabled = false;
        _isDead = true;
        OnPlayerDead?.Invoke(this);
    }
}
