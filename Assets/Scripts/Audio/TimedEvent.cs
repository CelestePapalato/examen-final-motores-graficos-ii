using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedEvent : MonoBehaviour
{
    public float time;

    public UnityEvent TimeOut;

    private void Start()
    {
        InvokeRepeating(nameof(callEvent), time, time);
    }

    private void callEvent()
    {
        TimeOut?.Invoke();
    }
}
