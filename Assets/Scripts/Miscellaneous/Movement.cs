using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, IBuffable
{
    [Header("Speed related")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float decceleration;
    [Header("Smoothing")]
    [SerializeField]
    [Range(0f, .5f)] float rotationSmoothing;

    private float speedBeforeRepose;
    private float speedMultiplier = 1f;
    private float currentMaxSpeed;
    private Vector2 input_vector = Vector2.zero;
    private Vector3 currentDirection = Vector3.zero;
    private Rigidbody rb;

    private float _rotationCurrentVelocity;

    public float SpeedMultiplier { get => speedMultiplier; set => speedMultiplier = value; }

    public float MaxSpeed
    {
        get => maxSpeed * speedMultiplier;
        set
        {
            if (value == maxSpeed) { return; }

            if (maxSpeed == 0)
            {
                maxSpeed = value;
                float diff = speedBeforeRepose / value;
                acceleration *= diff;
                decceleration *= diff;
            }
            else
            {
                if (value > 0)
                {
                    float diff = maxSpeed / value;
                    maxSpeed = value;
                    acceleration *= diff;
                    decceleration *= diff;
                    return;
                }
                if (value == 0)
                {
                    speedBeforeRepose = maxSpeed;
                    maxSpeed = 0;
                    return;
                }
            }
        }
    }
    public float Acceleration
    {
        get => acceleration * speedMultiplier; private set { acceleration = (value > 0) ? value : acceleration; }
    }
    public float Decceleration
    {
        get => decceleration * speedMultiplier; private set { decceleration = (value > 0) ? value : decceleration; }
    }

    public float RotationSmoothing
    {
        get => rotationSmoothing; set { rotationSmoothing = Mathf.Clamp(value, 0f, 0.5f); }
    }

    public float RigidbodySpeed { get => rb.velocity.magnitude; }
    public Rigidbody RigidBody { get => rb; }

    public Vector2 Direction
    {
        get => input_vector;

        set
        {
            if (value != Vector2.zero)
            {
                currentDirection.x = value.x;
                currentDirection.z = value.y;
                currentDirection.Normalize();
            }
            input_vector = Vector2.ClampMagnitude(value, 1);
        }

    }

    private void Awake()
    {
        speedBeforeRepose = maxSpeed;
        currentMaxSpeed = 0;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
        UpdateCurrentMaxSpeed();
        Move();
    }

    private void UpdateCurrentMaxSpeed()
    {
        float desiredSpeed = input_vector.magnitude * MaxSpeed;
        if (desiredSpeed < currentMaxSpeed)
        {
            currentMaxSpeed -= decceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentMaxSpeed += acceleration * Time.fixedDeltaTime;
        }
        currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0f, MaxSpeed);
    }

    void UpdateRotation()
    {
        if (input_vector != Vector2.zero)
        {

            float rotacionActual = transform.eulerAngles.y;
            float rotacionObjetivo = Mathf.Atan2(input_vector.x, input_vector.y) * Mathf.Rad2Deg;

            float rotacion = Mathf.SmoothDampAngle(rotacionActual, rotacionObjetivo, ref _rotationCurrentVelocity, rotationSmoothing);

            rb.MoveRotation(Quaternion.Euler(0f, rotacion, 0f));
        }
    }

    private void Move()
    {
        Vector3 targetSpeed = transform.forward * currentMaxSpeed;
        Vector3 currentSpeed = rb.velocity;
        currentSpeed.y = 0;
        Vector3 dv = targetSpeed - currentSpeed;
        float dt = Time.fixedDeltaTime;

        // a = v/t
        rb.AddForce(dv / dt, ForceMode.Acceleration);

        /*
        if (rb.velocity.magnitude > MaxSpeed)
        {
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0;
            Vector3 vel_objetivo = currentVelocity.normalized * MaxSpeed;
            Vector3 vel_excedida = currentVelocity - vel_objetivo;
            rb.AddForce(vel_excedida * -1, ForceMode.Acceleration);
        }
        */
    }

    public void Accept(IBuff buff)
    {
        if (buff == null) return;
        buff.Buff(this);
    }

    public void SpeedPowerUp(float multiplier, float time)
    {
        multiplier = Mathf.Max(multiplier, 1f);
        if (multiplier == 1)
        {
            return;
        }
        StopCoroutine(nameof(SpeedPowerUpEnabler));
        StartCoroutine(SpeedPowerUpEnabler(multiplier, time));
    }

    IEnumerator SpeedPowerUpEnabler(float multiplier, float time)
    {
        SpeedMultiplier = multiplier;
        yield return new WaitForSeconds(time);
        SpeedMultiplier = 1f;
    }

}