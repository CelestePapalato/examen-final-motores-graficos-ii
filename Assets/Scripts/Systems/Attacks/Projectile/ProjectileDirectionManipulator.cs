using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDirectionManipulator : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] float maxDegrees;

    Collider currentTarget;

    List<Collider> colliders = new List<Collider>();

    private void Start()
    {
        projectile = transform.parent.GetComponentInChildren<Projectile>();
    }

    private void FixedUpdate()
    {
        UpdatePath();
    }

    private void UpdatePath()
    {
        if (!projectile || !currentTarget) { return; }
        Vector3 target = currentTarget.transform.position - projectile.transform.position;
        Vector3 y_velocity = new Vector3(0, projectile.RB.velocity.y, 0);
        Vector3 new_velocity = target.normalized * projectile.Impulse;
        new_velocity.y = 0;
        projectile.RB.velocity = new_velocity + y_velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
        if(!currentTarget)
        {
            currentTarget = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
        if(currentTarget == other)
        {
            currentTarget = null;
        }
        if(colliders.Count > 0)
        {
            currentTarget = colliders[0];
        }
    }
}
