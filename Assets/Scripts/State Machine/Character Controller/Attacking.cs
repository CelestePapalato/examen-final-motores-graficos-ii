using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attacking : CharacterState, IAttacker
{
    bool attackBuffer;
    SkillData skillData;

    public void Setup(SkillData skill)
    {
        if (isActive) { return; }
        skillData = skill;
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        if (!skillData) {  return; }

        if(currentCharacter.Mana) {
            if (!currentCharacter.Mana.UseMana(skillData.ManaPoints))
            {
                currentCharacter.CambiarEstado(null);
                return;
            }
        }

        float y_velocity = currentCharacter.MovementComponent.RigidBody.velocity.y;
        //currentCharacter.MovementComponent.RigidBody.velocity = new Vector3(0f, y_velocity, 0f);
        attackBuffer = false;
        currentCharacter.Animator?.SetTrigger(skillData.AnimationTrigger);
        skillData.SetupCharacter(currentCharacter);
        StopPlayerActions();
        
        if (currentCharacter.AnimationEventHandler)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart += InitializeSettings;
            currentCharacter.AnimationEventHandler.onAnimationComplete += AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable += CanCombo;
            currentCharacter.AnimationEventHandler.onShoot += Shoot;
        }
    }

    public override void Salir()
    {
        ResumePlayerMovement();
        if (currentCharacter.AnimationEventHandler)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart -= InitializeSettings;
            currentCharacter.AnimationEventHandler.onAnimationComplete -= AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable -= CanCombo;
            currentCharacter.AnimationEventHandler.onShoot -= Shoot;
        }
        base.Salir();
    }

    private void OnEnable()
    {
        if (isActive && currentCharacter.AnimationEventHandler)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart += InitializeSettings;
            currentCharacter.AnimationEventHandler.onAnimationComplete += AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable += CanCombo;
            currentCharacter.AnimationEventHandler.onShoot += Shoot;
        }
    }

    private void OnDisable()
    {
        if (isActive && currentCharacter.AnimationEventHandler)
        {
            currentCharacter.AnimationEventHandler.onAnimationStart -= InitializeSettings;
            currentCharacter.AnimationEventHandler.onAnimationComplete -= AttackFinished;
            currentCharacter.AnimationEventHandler.onAnimationCancelable -= CanCombo;
            currentCharacter.AnimationEventHandler.onShoot -= Shoot;
        }
    }

    private void InitializeSettings(CharacterAnimatorState state)
    {
        if (state == CharacterAnimatorState.SKILL)
        {
            attackBuffer = false;
            canAttack = false;
            currentCharacter.MovementComponent.Impulse(skillData.OnStartImpulse);
        }
    }

    private void CanCombo(CharacterAnimatorState state)
    {
        if (state == CharacterAnimatorState.SKILL)
        {
            canAttack = true;
        }
    }

    private void AttackFinished(CharacterAnimatorState state)
    {
        if (attackBuffer)
        {
            return;
        }
        if (state == CharacterAnimatorState.SKILL)
        {
            personaje.CambiarEstado(null);
        }
    }

    public void Shoot()
    {
        if (currentCharacter.Shooter)
        {
            currentCharacter.Shooter.Shoot();
        }
    }

    public override void Move(Vector2 input)
    {
        currentCharacter.MovementComponent.Direction = input;
    }

    public override void Attack()
    {
        if(canAttack)
        {
            canAttack = false;
            currentCharacter.Animator?.SetTrigger("Attack");
            attackBuffer = true;
        }
    }
}
