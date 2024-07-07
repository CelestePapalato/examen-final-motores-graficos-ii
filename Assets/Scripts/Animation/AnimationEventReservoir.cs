using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterAnimatorState
{
    IDLE,
    ATTACK,
    STUN,
    DEAD
}

public class AnimationEventReservoir : MonoBehaviour
{
    public event Action<CharacterAnimatorState> onAnimationStart;
    public event Action<CharacterAnimatorState> onAnimationComplete;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AnimationStartHandler(CharacterAnimatorState state)
    {
        onAnimationStart?.Invoke(state);
    }

    public void AnimationCompleteHandler(CharacterAnimatorState state)
    {
        onAnimationComplete?.Invoke(state);
    }
}
