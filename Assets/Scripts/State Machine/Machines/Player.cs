using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : StateMachine
{
    Health healthComponent;
    Movement movement;
    PlayerController controller;
    PlayerInput playerInput;

    public UnityAction OnDead;
    bool attackInput = false;
    bool evadeInput = false;

    private float damageMultiplier = 1f;
    public float DamageMultiplier {  get { return damageMultiplier; } }

    protected override void Awake()
    {
        base.Awake();

        movement = GetComponent<Movement>();
        healthComponent = GetComponentInChildren<Health>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (healthComponent)
        {
            healthComponent.onDamaged += OnDamage;
            healthComponent.onNoHealth += Dead;
        }
    }

   private void OnDisable()
    {
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
        if (attackInput)
        {
            controller?.Attack();
        }
        if(evadeInput)
        {
            controller?.Evade();
        }
    }

    public override void CambiarEstado(Estado nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        controller = (PlayerController) estadoActual;
    }

    private void Dead()
    {
        movement.Direction = Vector2.zero;
        OnDead?.Invoke();
        playerInput.enabled = false;
        this.enabled = false;
    }
    private void OnMove(InputValue inputValue)
    {
        controller?.Move(inputValue);
    }

    private void OnAttack()
    {
        attackInput = !attackInput;
    }

    private void OnEvade()
    {
        evadeInput = !evadeInput;
    }

    private void OnDamage(int health, int maxHealth)
    {
        estadoActual?.DañoRecibido();
    }

}