using System;
using UnityEngine;

public enum CharacterAnimatorState
{
    IDLE,
    ATTACK,
    STUN,
    INTERACTION,
    DEAD
}

public class AnimationEventHandler : MonoBehaviour
{
    public event Action<CharacterAnimatorState> onAnimationStart;
    public event Action<CharacterAnimatorState> onAnimationComplete;
    public event Action<CharacterAnimatorState> onAnimationCancelable;

    public void AnimationStartHandler(CharacterAnimatorState state)
    {
        onAnimationStart?.Invoke(state);
    }

    public void AnimationCompleteHandler(CharacterAnimatorState state)
    {
        onAnimationComplete?.Invoke(state);
    }

    public void AnimationCancelableHandler(CharacterAnimatorState state)
    {
        onAnimationCancelable?.Invoke(state);
    }
}
