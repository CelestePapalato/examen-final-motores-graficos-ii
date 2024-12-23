using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterState : State
{
    [SerializeField] protected float maxSpeed;
    [SerializeField]
    [Tooltip(" -1 =  No changes")]
    protected float drag = -1;
    [SerializeField]
    [Range(0f, .5f)] float rotationSmoothing;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float evadeCooldown;

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

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        currentCharacter = personaje as Character;
        if (currentCharacter && currentCharacter.MovementComponent)
        {
            currentCharacter.MovementComponent.MaxSpeed = maxSpeed;
            currentCharacter.MovementComponent.Drag = drag;
            if(rotationSmoothing > 0)
            {
                currentCharacter.MovementComponent.RotationSmoothing = rotationSmoothing;
            }
        }
    }

    public override void Actualizar()
    {
        float speedFactor = (maxSpeed <= 0) ? 0 : currentCharacter.MovementComponent.RigidbodySpeed / maxSpeed;
        currentCharacter.Animator?.SetFloat("Speed", speedFactor);
    }

    public virtual void Move(Vector2 input)
    {        
        currentCharacter.MovementComponent.Direction = input;
    }

    public virtual void Attack()
    {
        if (!canAttack || attackCooldown <= 0) { return; }

        StopCoroutine(ControlAttackCooldown());
        StartCoroutine(ControlAttackCooldown());
    }

    public virtual void Evade()
    {
        if (!canEvade || evadeCooldown <= 0) { return; }
        StopCoroutine(ControlEvadeCooldown());
        StartCoroutine(ControlEvadeCooldown());
        currentCharacter.Animator?.SetTrigger("Evade");
    }

    public virtual void Interact() { }

    protected IEnumerator ControlAttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldownMultiplier * attackCooldown);
        canAttack = true;
    }

    protected IEnumerator ControlEvadeCooldown()
    {
        canEvade = false;
        yield return new WaitForSeconds(evadeCooldown);
        canEvade = true;
    }

    protected void StopPlayerActions()
    {
        canAttack = false;
        canEvade = false;
        // -> currentCharacter.MovementComponent.Direction = Vector2.zero;
        // currentCharacter.MovementComponent.RigidBody.velocity = new Vector3(0f, y_velocity, 0f);
    }

    protected void ResumePlayerMovement()
    {
        if (currentCharacter.Agent)
        {
            currentCharacter.Agent.enabled = false;
        }
        if (currentCharacter.MovementComponent)
        {
            currentCharacter.MovementComponent.enabled = true;
        }
    }

    protected void EnableRigidbody(bool enable)
    {
        if (currentCharacter.MovementComponent)
        {
            Rigidbody rb = currentCharacter.MovementComponent.RigidBody;
            if (!rb)
            {
                rb = currentCharacter.MovementComponent.GetComponent<Rigidbody>();
            }
            rb.isKinematic = !enable;
        }
    }

    protected void EnableAgent(bool enable)
    {
        if (currentCharacter.Agent)
        {
            currentCharacter.Agent.enabled = enable;
        }
    }
}
