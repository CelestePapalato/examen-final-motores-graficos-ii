using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLerp : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform transformStart;
    [SerializeField] Transform transformEnd;

    [Header("Configuration")]
    [SerializeField] bool position;
    [SerializeField] bool rotation;
    [SerializeField] bool useRigidbody;

    Rigidbody rb;
    private void Start()
    {
        if(!target) { target = transform; }

        if (useRigidbody)
        {
            rb = target.GetComponent<Rigidbody>();

            useRigidbody = rb != null;
        }

        Lerp(0);
    }

    public void Lerp(float value)
    {
        value = Mathf.Clamp01(value);

        if(position)
        {
            Vector3 lerpPosition = Vector3.Lerp(transformStart.position, transformEnd.position, value);
            if(useRigidbody)
            {
                rb.MovePosition(lerpPosition);
            }
            else
            {
                target.position = lerpPosition;
            }
        }
        if(rotation)
        {
            Quaternion lerpRotation = Quaternion.Lerp(transformStart.rotation, transformEnd.rotation, value);
            if (useRigidbody)
            {
                rb.MoveRotation(lerpRotation);
            }
            else
            {
                target.rotation = lerpRotation;
            }
        }
    }
}
