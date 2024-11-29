using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSystem
{
    public abstract class PuzzleInteractable : Puzzle, IInteractable
    {
        [SerializeField]
        protected bool isProlonguedInteraction = false;
        public bool ProlonguedInteraction { get => isProlonguedInteraction; }

        public virtual void Interact()
        {
            OnInteraction?.Invoke();
        }

        public virtual void StopInteraction()
        {
            if (!_completed) { OnInteractionHalted?.Invoke(); }
        }

        public UnityEvent OnInteraction;
        public UnityEvent OnInteractionHalted;

        public Puzzle Puzzle { get; protected set; }

        private void Awake()
        {
            Puzzle = this;
        }

        public float DistanceTo(Vector3 point)
        {
            return Vector3.Distance(transform.position, point);
        }
    }
}