using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected bool isProlonguedInteraction = false;
    public bool ProlonguedInteraction { get => isProlonguedInteraction; }

    public float DistanceTo(Vector3 point)
    {
        return Vector3.Distance(transform.position, point);
    }

    public void Interact()
    {

    }

    public void StopInteraction()
    {

    }

    public PuzzleSystem.Puzzle Puzzle {  get; private set; }

}
