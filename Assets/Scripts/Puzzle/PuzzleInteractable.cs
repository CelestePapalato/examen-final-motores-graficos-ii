using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSystem
{
    public abstract class PuzzleInteractable : Puzzle, IInteractable
    {       
        public abstract void Interact();
        public float DistanceTo(Vector3 point)
        {
            return Vector3.Distance(transform.position, point);
        }
    }
}