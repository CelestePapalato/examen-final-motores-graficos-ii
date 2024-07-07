using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Exploring : PlayerController
{
    [SerializeField] bool inputRelatedToCamera = true;
    [SerializeField] new Camera camera;

    protected override void Awake()
    {
        base.Awake();
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    public override void Actualizar()
    {
        animator.SetFloat("Speed", movement.RigidbodySpeed / maxSpeed);
    }

    public override void Move(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>(); 
        if(inputRelatedToCamera)
        {
            input = Quaternion.Euler(0f, 0f, -camera.transform.eulerAngles.y) * input;
        }
        movement.Direction = input;
        Debug.Log(input);
    }

    public override void Attack()
    {
        base.Attack();
    }

}
