using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Character : StateMachine
{
    [Header("States")]
    [SerializeField] CharacterController idleState;
    [SerializeField] CharacterController attackState;
    [SerializeField] CharacterController specialAttackState;
    [SerializeField] CharacterController stunState;
    [SerializeField] CharacterController interactionState;

    [Header("Hitboxes")]
    [SerializeField] bool disableHitboxesOnStart = true;
    
    [Header("Object Tracking")]
    [SerializeField] protected State estadoAlEncontrarObjetivo;
    [SerializeField] protected State estadoAlPerderObjetivo;

    Health health;
    Movement movement;
    CharacterController controller;
    Animator animator;
    Damage[] damage;
    Collider[] hitboxes;
    NavMeshAgent agent;
    IObjectTracker[] trackers;

    public Movement MovementComponent { get => movement; }
    public Health HealthComponent { get => health; }
    public Animator Animator { get => animator; }
    public Damage[] DamageComponents { get => damage;}

    public UnityAction OnDead;
    bool attackInput = false;
    bool evadeInput = false;

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; }

    private bool _isDead = false;

    public bool IsDead { get => _isDead; private set => _isDead = value; }

    public Transform CharacterTransform { get => movement.transform; }

    protected override void Awake()
    {
        firstState = idleState;

        base.Awake();

        movement = GetComponentInChildren<Movement>();
        health = GetComponentInChildren<Health>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        trackers = GetComponentsInChildren<IObjectTracker>();
    }

    private void Start()
    {
        if (disableHitboxesOnStart)
        {
            GetAllHitboxes(false);
        }
    }

    private void GetAllHitboxes(bool enable)
    {
        damage = GetComponentsInChildren<Damage>();
        hitboxes = new Collider[damage.Length];
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i] = damage[i].GetComponent<Collider>();
            hitboxes[i].enabled = enable;
        }
    }

    private void OnEnable()
    {
        if (health)
        {
            health.onDamaged += OnDamage;
            health.onNoHealth += Dead;
        }
    }

    private void OnDisable()
    {
        if (health)
        {
            health.onDamaged -= OnDamage;
            health.onNoHealth -= Dead;
        }
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        base.Update();
    }

    public override void CambiarEstado(State nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        controller = (CharacterController)currentState;
    }

    private void Dead()
    {
        movement.Direction = Vector2.zero;
        OnDead?.Invoke();
        this.enabled = false;
        _isDead = true;
    }
    public void Move(Vector2 input)
    {
        controller?.Move(input);
    }

    public void Attack()
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

    public void Evade()
    {
        evadeInput = !evadeInput;
        if (evadeInput)
        {
            controller?.Evade();
        }
    }
    public void Interact()
    {
        if (controller != interactionState)
        {
            controller?.Interact();
            CambiarEstado(interactionState);
        }
        else
        {
            controller?.Interact();
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


    public void TargetUpdate(Transform newTarget)
    {
        foreach (IObjectTracker tracker in trackers)
        {
            tracker.Target = newTarget;
        }

        if (newTarget)
        {
            CambiarEstado(estadoAlEncontrarObjetivo);
        }
        else
        {
            CambiarEstado(estadoAlPerderObjetivo);
        }
    }
}