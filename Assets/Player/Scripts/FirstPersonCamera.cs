using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float sensitivy;
    [SerializeField]
    [Tooltip("Sensitivity at lower values may be too much even for devices with lower DPI. Change the " +
        "proportion between the previous sentivity value and the one computed. Lower values are similar to " +
        "the original sensitivity.")]
    [Range(1, 10)]float sentivityProportion;

    [SerializeField]
    [Range(0f, 0.3f)] float smoothing;

    [Header("Angles")]
    [SerializeField]
    [Range(-90, 90)]float upperLookLimit;
    [SerializeField]
    [Range(-90, 90)] float lowerLookLimit;

    // Componentes
    Camera cam;
    Rigidbody rb;

    // Variables
    float rotacionEjeX;
    Vector2 suavidadV;

    Vector2 movimiento = Vector2.zero;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        rotacionEjeX = transform.localEulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        //movimiento = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        suavidadV.x = Mathf.SmoothStep(suavidadV.x, movimiento.x * sensitivy * 1/sentivityProportion, smoothing);
        suavidadV.y = Mathf.SmoothStep(suavidadV.y, movimiento.y * sensitivy * 1/sentivityProportion, smoothing);

        // Rotamos al jugador horizontalmente
        //transform.Rotate(Vector3.up, suavidadV.x);
        rb.MoveRotation(Quaternion.Euler(0f, suavidadV.x + rb.rotation.eulerAngles.y, 0f));

        // Rotamos verticalmente la cámara
        // 1 - Obtenemos la rotación objetivo de la cámara
        rotacionEjeX += suavidadV.y;

        // 2- Limitamos la rotación total a 90 grados
        rotacionEjeX = Mathf.Clamp(rotacionEjeX, lowerLookLimit, upperLookLimit);

        // 3- Rotamos en valores locales
        cam.transform.localRotation = Quaternion.Euler(-rotacionEjeX, 0, 0);

    }

    void OnCamera(InputValue inputValue)
    {
        movimiento = inputValue.Get<Vector2>();

    }
}