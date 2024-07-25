using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TargetDetection : MonoBehaviour
{
    [SerializeField] UnityEvent<Transform[]> TargetUpdate;
    [SerializeField] UnityEvent<Transform> TargetFound;
    [SerializeField] UnityEvent<Transform> TargetLost;

    List<Transform> targets = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if(targets.Contains(other.transform)) { return; }
        targets.Add(other.transform);
        TargetFound?.Invoke(other.transform);
        TargetUpdate?.Invoke(targets.ToArray());
    }

    private void OnTriggerExit(Collider collision)
    {
        targets.Remove(collision.transform);
        TargetLost?.Invoke(collision.transform);
        TargetUpdate?.Invoke(targets.ToArray());
    }
}
