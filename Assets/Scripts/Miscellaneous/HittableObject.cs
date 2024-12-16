using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour, IHittable
{
    [SerializeField]
    [Range(0f, 1f)] float impulseResistance;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Hit(IDamageDealer damageDealer)
    {
        Vector3 direction = transform.position - damageDealer.Position;
        direction.y = 0f;
        direction.Normalize();
        float factor = Mathf.Lerp(damageDealer.Impulse, 0, impulseResistance);
        rb.AddForce(direction * factor, ForceMode.Impulse);
    }
}
