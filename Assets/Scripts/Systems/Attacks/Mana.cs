using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mana : MonoBehaviour, IObservableVariable
{
    [SerializeField]
    private int maxMana;
    [SerializeField]
    private float manaRecoveryRate;

    private int currentMana;

    public UnityAction<int, int> OnManaUpdate;

    public event Action<int, int> OnUpdate;
    public event Action OnDestroyEvent;
    public int Current { get => currentMana; }
    public int Max { get => maxMana; }

    Coroutine ManaRecoveryCoroutine;

    void Start()
    {
        currentMana = maxMana;
        OnUpdate?.Invoke(currentMana, maxMana);
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
        OnDestroyEvent = null;
    }

    public bool UseMana(int points)
    {
        points = Mathf.Abs(points);
        if(currentMana < points)
        {
            return false;
        }

        currentMana -= points;
        OnManaUpdate?.Invoke(currentMana, maxMana);
        OnUpdate?.Invoke(currentMana, maxMana);
        StartRecovery();
        return true;
    }

    private void StartRecovery()
    {
        if (ManaRecoveryCoroutine != null)
        {
            StopCoroutine(ManaRecoveryCoroutine);
        }
        ManaRecoveryCoroutine = StartCoroutine(ManaRecovery());
    }

    private IEnumerator ManaRecovery()
    {
        while(currentMana < maxMana)
        {
            yield return new WaitForSeconds(manaRecoveryRate);
            currentMana++;
            OnManaUpdate?.Invoke(currentMana, maxMana);
            OnUpdate?.Invoke(currentMana, maxMana);
        }
    }
}
