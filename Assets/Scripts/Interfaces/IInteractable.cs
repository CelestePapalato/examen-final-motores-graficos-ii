using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public bool ProlonguedInteraction { get; }

    public void Interact();

    public void StopInteraction();

    public float DistanceTo(Vector3 point);

    public PuzzleSystem.Puzzle Puzzle {  get; }
}