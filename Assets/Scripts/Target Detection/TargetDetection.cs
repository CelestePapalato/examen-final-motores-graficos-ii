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
        Debug.Log("OnTriggerEnter");
        if(targets.Contains(other.transform)) { return; }
        targets.Add(other.transform);
        TargetUpdate?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("OnTriggerExit");
        targets.Remove(collision.transform);
        TargetUpdate?.Invoke(null);
    }
}
