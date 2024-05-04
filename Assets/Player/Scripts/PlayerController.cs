using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement constants")]
    [SerializeField]
    [Tooltip("This indicates the maximum unit/s the playerController will move with when accelerating while walking.")]
    float walkingMaxSpeed; [SerializeField]
    [Tooltip("This indicates the maximum unit/s the playerController will move with when accelerating while running.")]
    float runningMaxSpeed;
    [SerializeField]
    float airborneMaxSpeed;
    [SerializeField]
    [Tooltip("unit/s")]
    float walkingAcceleration;
    [SerializeField]
    [Tooltip("unit/s")]
    float runningAcceleration;
    [SerializeField]
    float airborneAcceleration;
    [SerializeField]
    [Tooltip("unit/s")]
    float deceleration;
    [SerializeField]
    [Tooltip("Maximum slope the player can walk onto.")]
    [Range(0f, 70f)]float maximumSlop = 45f;
    [SerializeField]
    [Tooltip("LayerMask that represents the ground. Used for checking if the player is on ground.")]
    LayerMask groundLayer;

    [Header("Input related")]
    [SerializeField]
    [Tooltip("Joystick axis zone where running speed values will be disabled after the player released the running button.")]
    [Range(0f, 0.9f)]float runningDeadZone;

    [Header("DEBUG")]
    [SerializeField]
    Text currentSpeedText;

    // Components
    Rigidbody rb;
    CapsuleCollider col;
    //CharacterController characterController;
    PlayerInput playerInput;

    PlayerControls playerControls;

    // Variables
    bool isRunning = false;
    bool isRunningButtonHeld = false;
    bool isOnGround = true;
    bool onGroundLastFrame = true;

    float currentAcceleration;
    float currentMaxSpeed;
    float bottomCapsuleOrigin; // transform.position.y - bottomCapsuleOrigin = la coordenada origen de la esfera inferior de la cápsula

    Vector2 movement_input = Vector2.zero;
    Vector3 groundNormal; // se usa para calcular el vector de movimiento cuando el
                          // jugador está en una superficie inclinada

    Dictionary<int, ContactPoint[]> contactPoints = new Dictionary<int, ContactPoint[]>();
    List<GameObject> collisions = new List<GameObject>();

    /*
     Broadcast Messages y Send Messages funcionan exactamente igual, siendo su única diferencia que
     Broadcast busca las funciones en los hijos y Send solamente en el GameObject donde el Player Input
     esté colocado.
     */

    private void Awake()
    {
        //characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();

        playerControls = new PlayerControls();

        currentAcceleration = walkingAcceleration;
        currentMaxSpeed = walkingMaxSpeed;


        bottomCapsuleOrigin = col.height * 0.5f - col.radius;
    }

    private void FixedUpdate()
    {
        //updateMovementInput();
        checkGroundState();
        Move();


        //DEBUG
        currentSpeedText.text = rb.velocity.ToString();

    }

    // MOVEMENT METHODS

    void Move()
    {
        if (movement_input.magnitude < runningDeadZone)
        {
            // Determinamos que el jugador quiere reducir la velocidad, por
            // lo que seteamos la aceleración y la velocidad máxima a sus valores
            // de caminata
            isRunning = false;
            updateMovementValues();
        }

        
        Vector3 forward = transform.forward;
        Vector3 movement = Vector3.zero;
        Vector3 targetSpeed;
        float difference;
        float _acceleration; // used for maths
        float _deceleration;

        if (isOnGround)
        {
            // Alineamos nuestro vector de movimiento a la normal.
            Vector3.OrthoNormalize(ref groundNormal, ref forward); // aseguramos que forward sea perpendicular a la normal del suelo

            // Vector3.Cross(groundNormal, forward) sería nuestra versión de transform.right, pero teniendo en cuenta,
            // de nuevo, la normal del terreno
            targetSpeed = Vector3.Cross(groundNormal, forward) * movement_input.x * currentMaxSpeed + forward * movement_input.y * currentMaxSpeed;

            _acceleration = currentAcceleration;
            _deceleration = -deceleration;
        }
        else
        {
            targetSpeed = (transform.right * movement_input.x + transform.forward * movement_input.y) * airborneMaxSpeed;
            targetSpeed.y = rb.velocity.y;

            _acceleration = airborneAcceleration;
            _deceleration = -airborneAcceleration;
        }

        difference = targetSpeed.magnitude - rb.velocity.magnitude;


        if (!Mathf.Approximately(difference, 0.0f)) // Si existe diferencia significativa entre nuestra velocidad objetivo y la actual
        {
            if (difference > 0) // Si es positiva (es decir, necesitamos más), entonces añadimos aceleración
            {
                _acceleration = Mathf.Min(_acceleration * Time.fixedDeltaTime, difference);
            }
            else
            {
                _acceleration = Mathf.Max(_deceleration * Time.fixedDeltaTime, difference);
            }

            difference = 1f / difference;
            movement = targetSpeed - rb.velocity;
            movement *= difference * _acceleration;
        }

        rb.velocity += movement;
        //rb.AddForce(movement, ForceMode.VelocityChange);
        onGroundLastFrame = isOnGround;
    }

    void updateMovementValues()
    {
        if (!isOnGround)
        {
            return;
        }
        if (isRunning)
        {
            currentAcceleration = runningAcceleration;
            currentMaxSpeed = runningMaxSpeed;
            return;
        }
        if (!isRunningButtonHeld)
        {
            currentAcceleration = walkingAcceleration;
            currentMaxSpeed = walkingMaxSpeed;
        }
    }

    private void checkGroundState()
    {
        RaycastHit hit;
        isOnGround = false;
        groundNormal = Vector3.zero;

        foreach (ContactPoint[] contacts in contactPoints.Values) // obtenemos los puntos de contacto
        {
            for(int i = 0; i < contacts.Length; i++)
            {
                Vector3 point = contacts[i].point;
                if (point.y <= rb.position.y - bottomCapsuleOrigin  //Verificamos que el punto está en el suelo 0.85f
                    && Physics.Raycast(point + Vector3.up, Vector3.down, out hit, 1.1f, groundLayer) // verificamos que el raycast golpee el suelo
                    && Vector3.Angle(hit.normal, Vector3.up) <= maximumSlop) // verificamos que el ángulo de la normal esté dentro del rango
                {
                    isOnGround = true;                    
                    groundNormal += hit.normal;
                }
            }
        }

        if (isOnGround) // normalizamos el vector solamente si hubo contacto
        {
            groundNormal.Normalize();
        }
    }

    // INPUT METHODS

    private void OnMove(InputValue inputValue)
    {
        movement_input = inputValue.Get<Vector2>();
    }

    private void OnRun()
    {        
        if (isRunningButtonHeld)
        {
            isRunning = false;
            isRunningButtonHeld = false;
            Debug.Log("Released Running");
        }
        else
        {
            isRunning = true;
            isRunningButtonHeld = true;
            updateMovementValues();
            Debug.Log("Running");
        }
    }

    // COLLISION METHODS

    private void OnCollisionEnter(Collision collision)
    {
        // este código es para realizar un seguimiento de la colisión y los puntos de contacto
        collisions.Add(collision.gameObject);
        contactPoints.Add(collision.gameObject.GetInstanceID(), collision.contacts);
    }

    void OnCollisionStay(Collision collision)
    {
        // Actualizamos los puntos de contacto del objeto con el que aún estamos colisionando
        contactPoints[collision.gameObject.GetInstanceID()] = collision.contacts;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisions.RemoveAll(x => x.gameObject == collision.gameObject);
        contactPoints.Remove(collision.gameObject.GetInstanceID());
    }
}
