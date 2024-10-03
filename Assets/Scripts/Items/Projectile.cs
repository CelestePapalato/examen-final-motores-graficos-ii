using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    float impulse;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        impulse = Mathf.Max(impulse, 0.0f);
    }

    void Start()
    {
        rb.AddForce(transform.forward * impulse, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
    }
}
