using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterState : State
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


    public IInteractable currentInteractable;

    protected Character currentCharacter;

    protected virtual void Awake()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    protected float GetPlayerDamageMultiplier()
    {
        return (currentCharacter) ? currentCharacter.DamageMultiplier : 1f;
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        currentCharacter = personaje as Character;
        if (currentCharacter && currentCharacter.MovementComponent)
        {
            currentCharacter.MovementComponent.MaxSpeed = maxSpeed;
        }
    }

    public override void Actualizar()
    {
        float speedFactor = (maxSpeed <= 0) ? 0 : currentCharacter.MovementComponent.RigidbodySpeed / maxSpeed;
        currentCharacter.Animator?.SetFloat("Speed", speedFactor);
    }

    public virtual void Move(Vector2 input)
    {
        if (inputRelatedToCamera)
        {
            input = Quaternion.Euler(0f, 0f, -camera.transform.eulerAngles.y) * input;
        }
        currentCharacter.MovementComponent.Direction = input;
    }

    public virtual void Attack()
    {
        if (!canAttack || attackCooldown <= 0) { return; }

        StopCoroutine(ControlAttackCooldown());
        StartCoroutine(ControlAttackCooldown());
        currentCharacter.Animator?.SetTrigger("Attack");
    }

    public virtual void Evade()
    {
        if (!canEvade || evadeCooldown <= 0) { return; }
        StopCoroutine(ControlEvadeCooldown());
        StartCoroutine(ControlEvadeCooldown());
        currentCharacter.Animator?.SetTrigger("Evade");
    }

    public virtual void Interact() { }

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

    protected void StopPlayerMovement()
    {
        canAttack = false;
        canEvade = false;
        float y_velocity = currentCharacter.MovementComponent.RigidBody.velocity.y;
        currentCharacter.MovementComponent.RigidBody.velocity = new Vector3(0f, y_velocity, 0f);
    }

    protected void ResumePlayerMovement()
    {
        currentCharacter.MovementComponent.enabled = true;
    }
}
