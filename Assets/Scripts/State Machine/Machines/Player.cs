using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : StateMachine
{
    private static List<Player> currentPlayers = new List<Player>();
    public static Player[] CurrentPlayers {  get { return currentPlayers.ToArray(); } }

    public static event Action<Player> OnPlayerDead;

    [Header("States")]
    [SerializeField] CharacterController idleState;
    [SerializeField] CharacterController attackState;
    [SerializeField] CharacterController specialAttackState;
    [SerializeField] CharacterController stunState;
    [SerializeField] CharacterController interactionState;

    Health healthComponent;
    Movement movement;
    CharacterController controller;
    Animator animator;
    PlayerInput playerInput;
    Collider[] hitboxes;

    public UnityAction OnDead;
    bool attackInput = false;
    bool evadeInput = false;

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; }

    private bool _isDead = false;

    public bool IsDead { get => _isDead; private set => _isDead = value; }

    protected override void Awake()
    {
        firstState = idleState;

        base.Awake();

        movement = GetComponentInChildren<Movement>();
        healthComponent = GetComponentInChildren<Health>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        GetAllHitboxes(false);
    }

    private void GetAllHitboxes(bool enable)
    {
        Damage[] damageComponents = GetComponentsInChildren<Damage>();
        hitboxes = new Collider[damageComponents.Length];
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i] = damageComponents[i].GetComponent<Collider>();
            hitboxes[i].enabled = enable;
        }
    }

    private void OnEnable()
    {
        currentPlayers.Add(this);
        if (healthComponent)
        {
            healthComponent.onDamaged += OnDamage;
            healthComponent.onNoHealth += Dead;
        }
    }

   private void OnDisable()
    {
        currentPlayers.Remove(this);
        if (healthComponent)
        {
            healthComponent.onDamaged -= OnDamage;
            healthComponent.onNoHealth -= Dead;
        }
    }

    protected override void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }
        base.Update();
    }

    public override void CambiarEstado(State nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        controller = (CharacterController) currentState;
    }

    private void Dead()
    {
        movement.Direction = Vector2.zero;
        OnDead?.Invoke();
        playerInput.enabled = false;
        this.enabled = false;
        _isDead = true;
        OnPlayerDead?.Invoke(this);
    }
    private void OnMove(InputValue inputValue)
    {
        controller?.Move(inputValue);
    }

    private void OnAttack()
    {
        attackInput = !attackInput;
        if (attackInput)
        {
            controller?.Attack();
        }
        if (currentState == idleState && attackState)
        {
            CambiarEstado(attackState);
        }
    }

    private void OnEvade()
    {
        evadeInput = !evadeInput;
        if (evadeInput)
        {
            controller?.Evade();
        }
    }

    private void OnDamage(int health, int maxHealth)
    {
        Debug.Log("Vida: " + health);
        currentState?.Da�oRecibido();
        if (stunState)
        {            
            CambiarEstado(stunState);
        }
        animator?.SetTrigger("Damage");
    }

    private void OnInteract()
    {   
        if(controller != interactionState)
        {
            controller?.Interact();
            CambiarEstado(interactionState);
        }
        else
        {
            controller?.Interact();
        }
    }
}