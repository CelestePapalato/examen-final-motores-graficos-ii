using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, IBuffable, IHittable
{
    [Header("Floor")]
    [Tooltip("Inclinación máxima del terreno para considerar que el objeto está en el suelo")]
    [Range(0f, 90f)] float maxSlope = 45f;
    [SerializeField] LayerMask floorLayer;
    [Header("Speed related")]
    [Tooltip("Drag afecta a la velocidad máxima")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float decceleration;
    [SerializeField] float drag;
    [Header("Smoothing")]
    [SerializeField]
    [Range(0f, .5f)] float rotationSmoothing;
    [Header("Impulse")]
    [SerializeField]
    [Range(0f, 1f)] float impulseResistance;

    private float speedBeforeRepose;
    private float speedMultiplier = 1f;
    private float currentMaxSpeed;
    private Vector2 input_vector = Vector2.zero;
    private Vector3 currentDirection = Vector3.zero;
    private Rigidbody rb;

    private float _rotationCurrentVelocity;

    Dictionary<int, ContactPoint[]> contactPoints = new Dictionary<int, ContactPoint[]>();

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
                acceleration /= diff;
                decceleration /= diff;
            }
            else
            {
                if (value > 0)
                {
                    float diff = maxSpeed / value;
                    maxSpeed = value;
                    acceleration /= diff;
                    decceleration /= diff;
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

    private float og_drag = -1;
    public float Drag
    {
        get => drag; set { drag = (value >= 0) ? value : og_drag; }
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

    public bool UpdateRotationON = true;
    public bool UpdatePositionON = true;

    public bool OnFloor {  get; private set; }

    private Vector3 groundNormal = Vector3.zero;

    private void Awake()
    {
        speedBeforeRepose = maxSpeed;
        currentMaxSpeed = 0;
        rb = GetComponent<Rigidbody>();
        og_drag = drag;
    }

    private void FixedUpdate()
    {
        CheckGroundState();
        UpdateRotation();
        UpdateCurrentMaxSpeed();
        Move();
    }

    public void CheckGroundState()
    {
        groundNormal = Vector3.zero;
        OnFloor = false;
        foreach (ContactPoint[] contacts in contactPoints.Values) // obtenemos los puntos de contacto
        {
            for (int i = 0; i < contacts.Length; i++)
            {
                OnFloor = OnFloor || contacts[i].normal.y >= Mathf.Cos(maxSlope * Mathf.Deg2Rad);
                groundNormal += contacts[i].normal;
            }
        }
    }

    private void UpdateCurrentMaxSpeed()
    {
        float desiredSpeed = input_vector.magnitude * MaxSpeed;
        if (desiredSpeed < currentMaxSpeed)
        {
            currentMaxSpeed -= Decceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentMaxSpeed += Acceleration * Time.fixedDeltaTime;
        }
        currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0f, MaxSpeed);
    }

    void UpdateRotation()
    {
        if(!UpdateRotationON) { return; }
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
        Vector3 forward = transform.forward;

        Vector3 velocity = rb.velocity;
        Vector3 normal = Vector3.up;
        float speed = velocity.magnitude;
        Vector3.OrthoNormalize(ref normal, ref velocity); // velocity pasa a ser un versor
        velocity.y = 0f;

        if (OnFloor)
        {
            Vector3.OrthoNormalize(ref groundNormal, ref forward);
            Vector3.OrthoNormalize(ref groundNormal, ref velocity);
        }

        velocity *= speed;
        float dragMagnitude = velocity.sqrMagnitude * drag;
        Vector3 dragVector = -velocity.normalized * dragMagnitude;

        if (UpdatePositionON)
        {
            Vector3 desiredVelocity = forward * currentMaxSpeed * input_vector.magnitude;
            if (speed > MaxSpeed * 3 / 5)
            {
                desiredVelocity *= Mathf.InverseLerp(MaxSpeed, 0, speed);
            }

            float dt = Time.fixedDeltaTime;


            if (!desiredVelocity.IsNaN())
            {
                rb.AddForce(desiredVelocity / dt, ForceMode.Acceleration);
            }
        }

        if (dragVector != null)
        {
            rb.AddForce(dragVector);
        }

        /*
        Vector3 targetSpeed = transform.forward * currentMaxSpeed;
        Vector3 currentSpeed = rb.velocity;
        currentSpeed.y = 0;
        Vector3 dv = targetSpeed - currentSpeed;
        float dt = Time.fixedDeltaTime;

        // a = v/t
        rb.AddForce(dv / dt, ForceMode.Acceleration);
        */
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
        SpeedMultiplier = multiplier;
        CancelInvoke(nameof(SpeedPowerUpDisabler));
        Invoke(nameof(SpeedPowerUpDisabler), time);
    }

    private void SpeedPowerUpDisabler()
    {
        SpeedMultiplier = 1;
    }

    public void Hit(IDamageDealer damageDealer)
    {
        Vector3 direction = transform.position - damageDealer.Position;
        direction.y = 0f;
        direction.Normalize();
        Vector3.OrthoNormalize(ref groundNormal, ref direction);
        float factor = Mathf.Lerp(damageDealer.Impulse, 0, impulseResistance);
        rb.AddForce(direction * factor, ForceMode.Impulse);
        /*
        AddImpulse(damageDealer.Impulse * direction);
        */
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != floorLayer) { return; }
        Debug.Log(collision.gameObject.name);
        contactPoints.Add(collision.gameObject.GetInstanceID(), collision.contacts);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != floorLayer) { return; }
        contactPoints[collision.gameObject.GetInstanceID()] = collision.contacts;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != floorLayer) { return; }
        contactPoints.Remove(collision.gameObject.GetInstanceID());
    }
}