using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Character : StateMachine
{
    [Header("Skills")]
    [SerializeField] SkillData NormalAttackData;
    [SerializeField] SkillData SpecialAttackData;
    public Transform SkillSpawnPoint {  get; private set; }

    [Header("States")]
    [SerializeField] CharacterState idleState;
    [SerializeField] CharacterState attackState;
    [SerializeField] CharacterState stunState;
    [SerializeField] CharacterState interactionState;
    [SerializeField] CharacterState deadState;
    
    [Header("Object Tracking")]
    [SerializeField] protected CharacterState followState;

    [Header("Hitboxes")]
    [SerializeField] bool disableHitboxesOnStart = true;

    Health health;
    Movement movement;
    CharacterState controller;
    Animator animator;
    Damage[] damage;
    Collider[] hitboxes;
    NavMeshAgent agent;
    AnimationEventHandler animEvent;
    IObjectTracker[] trackers;
    IAttacker[] attackers;

    public Movement MovementComponent { get => movement; }
    public NavMeshAgent Agent { get => agent; }
    public Health HealthComponent { get => health; }
    public Animator Animator { get => animator; }
    public Damage[] DamageComponents { get => damage;}
    public AnimationEventHandler AnimationEventHandler { get => animEvent;}

    public UnityAction OnDead;
    bool attackInput = false;
    bool evadeInput = false;

    private bool _isDead = false;

    public bool IsDead { get => _isDead; private set => _isDead = value; }

    public Transform CharacterTransform { get => movement.transform; }

    CharacterState currentIdleState;

    private bool toBeDestroyed = false;

    protected override void Awake()
    {
        movement = GetComponentInChildren<Movement>();
        health = GetComponentInChildren<Health>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();

        trackers = GetComponentsInChildren<IObjectTracker>();
        attackers = GetComponentsInChildren<IAttacker>();

        animEvent = GetComponentInChildren<AnimationEventHandler>();

        firstState = idleState;
        currentIdleState = idleState;

        base.Awake();
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
            damage[i].Owner = this;
            hitboxes[i] = damage[i].GetComponent<Collider>();
            hitboxes[i].enabled = enable;
        }
    }

    protected virtual void OnEnable()
    {
        if (health)
        {
            health.OnDamaged += OnDamage;
            health.OnDead += Dead;
        }
    }

    protected virtual void OnDisable()
    {
        if (health)
        {
            health.OnDamaged -= OnDamage;
            health.OnDead -= Dead;
        }
    }

    private void OnDestroy()
    {
        toBeDestroyed = true;   
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        base.Update();
    }

    public override void CambiarEstado(State newState)
    {
        currentState?.Salir();
        if (!toBeDestroyed)
        {
            currentState = (newState) ? newState : currentIdleState;
            currentState?.Entrar(this);
            controller = (CharacterState)currentState;
        }
    }

    protected virtual void Dead()
    {
        Debug.Log("xddd");
        if (deadState) { CambiarEstado(deadState); }
        OnDead?.Invoke();
        this.enabled = false;
        _isDead = true;
    }
    public void Move(Vector2 input)
    {
        controller?.Move(input);
    }

    public virtual void Attack()
    {
        Attack(NormalAttackData);
    }

    protected virtual void Attack(SkillData skillData)
    {
        attackInput = !attackInput;
        if (!skillData) { return; }
        if (attackInput)
        {
            controller?.Attack();
        }
        if (currentState == currentIdleState && attackState)
        {
            foreach (IAttacker attacker in attackers)
            {
                attacker.Setup(skillData);
            }
            CambiarEstado(attackState);
        }
    }

    public virtual void SpecialAttack()
    {
        Attack(SpecialAttackData);
    }

    public virtual void Evade()
    {
        evadeInput = !evadeInput;
        if (evadeInput)
        {
            controller?.Evade();
        }
    }
    public virtual void Interact()
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

    public virtual void MoveTowards(Transform target)
    {
        foreach (IObjectTracker tracker in trackers)
        {
            tracker.Target = target;
        }

        if (target)
        {
            currentIdleState = followState;
        }
        else
        {
            currentIdleState = idleState;
        }
        CambiarEstado(currentIdleState);
    }

    private void OnDamage(int health, int maxHealth)
    {
        Debug.Log("Vida: " + health);
        currentState?.DañoRecibido();
        if (stunState)
        {
            CambiarEstado(stunState);
        }
        animator?.SetTrigger("Damage");
    }

}