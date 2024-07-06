using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFollower : MonoBehaviour
{
    [Header("Seguimiento al jugador")]
    [SerializeField]
    [Range(0f, 0.3f)] float _movementSmoothing;
    [Header("Rotación")]
    [SerializeField]
    [Range(0f, 0.3f)] float rotationSmoothing;
    [SerializeField]
    float minAnguloVertical;
    [SerializeField]
    float maxAnguloVertical;

    float rotacionVerticalActual = 0f;
    float rotacionHorizontalActual = 0f;
    Vector2 smoothing_currentVelocty;
    Vector2 startPos;
    Vector2 endPos;
    GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _player.transform.position, (1f / _movementSmoothing) * Time.deltaTime);      
    }

    private void OnCamera(InputValue inputValue)
    {
        /*
        Vector2 rotationDelta = inputValue.Get<Vector2>();
        Vector2 offset = rotationDelta * Time.deltaTime;

        startPos = new Vector2(rotacionHorizontalActual, rotacionVerticalActual);
        endPos = endPos + offset;
        endPos.y = Mathf.Clamp(endPos.y, minAnguloVertical, maxAnguloVertical);

        startPos = Vector2.SmoothDamp(startPos, endPos, ref smoothing_currentVelocty, rotationSmoothing);

        rotacionHorizontalActual = startPos.x;
        rotacionVerticalActual = startPos.y;

        transform.localRotation = Quaternion.Euler(rotacionVerticalActual, rotacionHorizontalActual, 0f);
        */
    }
}