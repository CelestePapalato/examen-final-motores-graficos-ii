using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter.Invoke();
    }
}
