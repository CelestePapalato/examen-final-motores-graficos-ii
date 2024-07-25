using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TargetDetection : MonoBehaviour
{
    [SerializeField] UnityEvent<Transform> TargetUpdate;

    List<Transform> targets = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if(targets.Contains(other.transform)) { return; }
        targets.Add(other.transform);
        TargetUpdate?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider collision)
    {
        targets.Remove(collision.transform);
        TargetUpdate?.Invoke(null);
    }
}
