using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterController : State
{
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float evadeCooldown;

    [Header("Camera")]
    [SerializeField] protected bool inputRelatedToCamera;
    [SerializeField] protected new Camera camera;

    // # MOVEMENT ====================
    public float MaxSpeed { get => maxSpeed; }
    // =============================== #

    // # ATTACK ======================
    protected bool canAttack = true;

    protected float attackCooldownMultiplier = 1;

    public float AttackCooldownMultiplier
    {
        get => attackCooldownMultiplier;
        set => attackCooldownMultiplier = (value <= 1) ? value : attackCooldownMultiplier;
    }
    // =============================== # 

    // # EVADE =======================
    protected bool canEvade = true;

    public bool CanEvade { get => canEvade; set => canEvade = value; }
    // =============================== #

    protected bool isActive = false;

    protected Movement movement;
    protected Animator animator;
    protected AnimationEventHandler animEvent;

    protected virtual void Awake()
    {
        movement = GetComponentInChildren<Movement>();
        animator = GetComponentInChildren<Animator>();
        animEvent = GetComponentInChildren<AnimationEventHandler>();
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    protected float GetPlayerDamageMultiplier()
    {
        Player player = personaje as Player;
        if (player)
        {
            return player.DamageMultiplier;
        }
        else
        {
            return 1f;
        }
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        isActive = true;
        if (movement)
        {
            movement.MaxSpeed = maxSpeed;
        }
    }

    public override void Salir()
    {
        base.Salir();
        isActive = false;
    }

    public virtual void Move(InputValue inputValue) { }

    public virtual void Attack()
    {
        if (!canAttack || attackCooldown <= 0) { return; }

        StopCoroutine(ControlAttackCooldown());
        StartCoroutine(ControlAttackCooldown());
        animator?.SetTrigger("Attack");
    }

    public virtual void Evade()
    {
        if (!canEvade || evadeCooldown <= 0) { return; }
        StopCoroutine(ControlEvadeCooldown());
        StartCoroutine(ControlEvadeCooldown());
        animator?.SetTrigger("Evade");
    }

    IEnumerator ControlAttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldownMultiplier * attackCooldown);
        canAttack = true;
    }

    IEnumerator ControlEvadeCooldown()
    {
        canEvade = false;
        yield return new WaitForSeconds(evadeCooldown);
        canEvade = true;
    }
}
