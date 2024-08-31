using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private static List<Player> currentPlayers = new List<Player>();
    public static Player[] CurrentPlayers {  get { return currentPlayers.ToArray(); } }

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
            if(currentPlayers.Count == 0) { return null; }

            Player[] alivePlayers = currentPlayers.Where(x => !x.IsDead).ToArray();
            int r = UnityEngine.Random.Range(0, alivePlayers.Length);
            return alivePlayers[r];
        }
    }

    public static event Action<Player> OnPlayerDead;

    Healthbar healthBar;
    Character chara;

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; }

    private bool _isDead = false;

    public bool IsDead { get => _isDead; private set => _isDead = value; }

    public Transform CharacterTransform { get => (chara) ? chara.MovementComponent.transform : transform; }

    public Character PlayerCharacter { get => chara; }

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
        OnMove(null);
        this.enabled = false;
        _isDead = true;
        OnPlayerDead?.Invoke(this);
    }
    private void OnMove(InputValue inputValue)
    {
        Vector2 input = (inputValue != null) ? inputValue.Get<Vector2>() : Vector2.zero;
        chara?.Move(input);
    }

    private void OnAttack()
    {
        chara?.Attack();
    }

    private void OnSpecialAttack()
    {
        chara?.SpecialAttack();
    }

    private void OnEvade()
    {
        chara?.Evade();
    }

    private void OnInteract()
    {   
        chara?.Interact();
    }
}