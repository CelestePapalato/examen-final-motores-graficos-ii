using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
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
