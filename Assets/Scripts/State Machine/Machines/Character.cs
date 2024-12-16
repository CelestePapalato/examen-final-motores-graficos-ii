using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Character : StateMachine
{
    [Header("Damage Resistance")]
    [SerializeField] int hitCountResistance = 1;
    [SerializeField] float hitTimePeriod = 1f;

    [Header("Skills")]
    [SerializeField] SkillData NormalAttackData;
    [SerializeField] SkillData SpecialAttackData;
    [SerializeField] SkillData EvadeData;
    public Transform SkillSpawnPoint {  get; private set; }

    [Header("States")]
    [SerializeField] CharacterState idleState;
    [SerializeField] CharacterState skillState;
    [SerializeField] CharacterState patrolState;
    [SerializeField] CharacterState stunState;
    [SerializeField] CharacterState interactionState;
    [SerializeField] CharacterState deadState;
    
    [Header("Object Tracking")]
    [SerializeField] protected CharacterState followState;

    [Header("Hitboxes")]
    [SerializeField] bool disableHitboxesOnStart = true;

    Health health;
    Mana mana;
    Movement movement;
    CharacterState controller;
    Animator animator;
    Damage[] damage;
    ProjectileShooter shooter;
    Collider[] hitboxes;
    NavMeshAgent agent;
    AnimationEventHandler animEvent;
    IObjectTracker[] trackers;
    IAvoidObject[] avoiders;
    IAttacker[] attackers;

    public Movement MovementComponent { get => movement; }
    public NavMeshAgent Agent { get => agent; }
    public Health Health { get => health; }
    public Mana Mana { get => mana; }
    public Animator Animator { get => animator; }
    public Damage[] DamageComponents { get => damage;}
    public ProjectileShooter Shooter { get => shooter; }
    public AnimationEventHandler AnimationEventHandler { get => animEvent;}

    public UnityAction OnDead;
    public UnityAction OnResurrection;
    public UnityAction OnStunStart;
    public UnityAction OnStunEnd;
    bool attackInput = false;

    private bool _isDead = false;

    public bool IsDead { get => _isDead; private set => _isDead = value; }

    public Transform CharacterTransform { get => movement.transform; }

    CharacterState currentIdleState;

    private bool toBeDestroyed = false;
    public bool DisableStateChange = false;

    int currentHitCount = 0;
    bool hitMemoryCoroutineON = false;

    protected override void Awake()
    {
        movement = GetComponentInChildren<Movement>();
        health = GetComponentInChildren<Health>();
        mana = GetComponentInChildren<Mana>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();

        trackers = GetComponentsInChildren<IObjectTracker>();
        avoiders = GetComponentsInChildren<IAvoidObject>();
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
        shooter = GetComponentInChildren<ProjectileShooter>();
        if(shooter != null) { shooter.Owner = this; }
    }

    protected virtual void OnEnable()
    {
        if (health)
        {
            health.OnDamaged += OnDamage;
            health.OnDead += Dead;
            health.OnRevive.AddListener(Revive);
        }
    }

    protected virtual void OnDisable()
    {
        if (health)
        {
            health.OnDamaged -= OnDamage;
            health.OnDead -= Dead;
            health.OnRevive?.RemoveListener(Revive);
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
        if(DisableStateChange) { return; }
        bool damaged = currentState == stunState;
        currentState?.Salir();
        if (!toBeDestroyed)
        {
            currentState = (newState) ? newState : currentIdleState;
            currentState?.Entrar(this);
            controller = (CharacterState)currentState;
            if (damaged)
            {
                OnStunEnd?.Invoke();
            }
            return;
        }
        currentState = null;
        idleState = null;
    }

    protected virtual void Dead()
    {
        if (deadState) { CambiarEstado(deadState); }
        OnDead?.Invoke();
        _isDead = true;
    }

    protected virtual void Revive()
    {
        if (_isDead)
        {
            _isDead = false;
            CambiarEstado(currentIdleState);
            animator.SetTrigger("Revive");
            OnResurrection?.Invoke();
        }
    }

    public void Move(Vector2 input)
    {
        controller?.Move(input);
    }

    public virtual void Attack()
    {
        Skill(NormalAttackData);
    }

    public virtual void Evade()
    {
        Skill(EvadeData);
    }

    protected virtual void Skill(SkillData skillData)
    {
        attackInput = !attackInput;
        if (!skillData) { return; }
        if (attackInput)
        {
            controller?.Attack();
        }
        if (currentState == currentIdleState && skillState)
        {
            foreach (IAttacker attacker in attackers)
            {
                attacker.Setup(skillData);
            }
            CambiarEstado(skillState);
        }
    }

    public virtual void SpecialAttack()
    {
        Skill(SpecialAttackData);
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

    public virtual void Avoid(Transform target)
    {
        foreach(IAvoidObject avoider in avoiders)
        {
            avoider.AvoidTransform(target);
        }
    }

    public virtual void StopAvoiding(Transform target)
    {
        foreach(IAvoidObject avoider in avoiders)
        {
            avoider.StopAvoiding(target);
        }
    }

    public virtual void StartPatrol()
    {
        CambiarEstado(patrolState);
        currentIdleState = patrolState;
    }

    public virtual void EndPatrol()
    {
        CambiarEstado(idleState);
        currentIdleState = idleState;
    }

    private void OnDamage(int health, int maxHealth)
    {
        currentHitCount++;
        if (!hitMemoryCoroutineON)
        {
            StartCoroutine(HitCountMemory());
        }
    }

    private void EnterDamageState()
    {
        currentState?.DañoRecibido();
        if (stunState)
        {
            CambiarEstado(stunState);
        }
        animator?.SetTrigger("Damage");
        OnStunStart?.Invoke();
    }

    IEnumerator HitCountMemory()
    {
        float t = 0;
        while(t <= hitTimePeriod)
        {
            yield return null;
            t += Time.deltaTime;
            if(currentHitCount >= hitCountResistance)
            {
                EnterDamageState();
                break;
            }
        }
        currentHitCount = 0;
    }
}