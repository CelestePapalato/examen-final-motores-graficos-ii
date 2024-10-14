using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    float impulse;

    Rigidbody rb;

    public float Impulse { get => impulse; }
    public Rigidbody RB { get => rb; }

    Damage damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damage = GetComponentInChildren<Damage>();
        impulse = Mathf.Max(impulse, 0.0f);
    }

    private void OnEnable()
    {
        damage.DamageDealed += DestroyGameObject;
    }

    private void OnDisable()
    {
        damage.DamageDealed -= DestroyGameObject;
    }

    void Start()
    {
        rb.velocity = transform.forward * impulse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}