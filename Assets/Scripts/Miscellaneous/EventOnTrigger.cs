using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;
    public UnityEvent SomeoneInside;
    public UnityEvent NoOneInside;

    List<Collider> inside = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        bool noOne = inside.Count == 0;
        TriggerEnter?.Invoke();
        inside.Add(other);
        if(noOne) { SomeoneInside?.Invoke(); }
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke();
        inside.Remove(other);
        if(inside.Count == 0) { NoOneInside?.Invoke(); }
    }

    private void OnDisable()
    {
        inside.Clear();
    }
}
