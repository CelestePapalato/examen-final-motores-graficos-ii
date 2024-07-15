using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacting : CharacterController
{
    PuzzleSystem.Puzzle currentPuzzle;

    private void CheckForInteractables()
    {
        IInteractable nearest = null;
        float distance = Mathf.Infinity;
        Collider[] collisions = Physics.OverlapSphere(movement.transform.position, 2, LayerMask.GetMask("Interactable"));
        foreach (Collider col in collisions)
        {
            IInteractable interactable = col.gameObject.GetComponent<IInteractable>();
            if (interactable != null && (nearest == null || distance > interactable.DistanceTo(transform.position)))
            {
                nearest = interactable;
                distance = nearest.DistanceTo(transform.position);
            }
        }
        currentInteractable = nearest;
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        CheckForInteractables();
        Debug.Log(currentInteractable);
        if (currentInteractable == null)
        {
            personaje.CambiarEstado(null);
            return;
        }
        StopPlayerMovement();
        currentInteractable.Interact();
        currentPuzzle = currentInteractable.Puzzle; if (currentPuzzle != null)
        {
            currentPuzzle.PuzzleStateUpdated.AddListener(Interact);
        }
    }

    private void OnDisable()
    {
        if (currentPuzzle != null)
        {
            currentPuzzle.PuzzleStateUpdated?.RemoveListener(Interact);
        }
    }

    private void OnEnable()
    {
        if(currentPuzzle != null)
        {
            currentPuzzle.PuzzleStateUpdated.AddListener(Interact);
        }
    }

    public override void Salir()
    {
        currentInteractable?.StopInteraction();
        currentInteractable = null;
        if (currentPuzzle != null)
        {
            currentPuzzle.PuzzleStateUpdated?.RemoveListener(Interact);
            currentPuzzle = null;
        }
        ResumePlayerMovement();
        base.Salir();
    }

    public override void Interact()
    {
        personaje.CambiarEstado(null);
    }

}
