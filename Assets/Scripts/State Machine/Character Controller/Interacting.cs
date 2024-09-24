using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacting : CharacterState
{
    PuzzleSystem.Puzzle currentPuzzle;

    private void CheckForInteractables()
    {
        IInteractable nearest = null;
        float distance = Mathf.Infinity;
        Collider[] collisions = Physics.OverlapSphere(currentCharacter.MovementComponent.transform.position, 2, LayerMask.GetMask("Interactable"));
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
        currentCharacter = personajeActual as Character;
        CheckForInteractables();
        Debug.Log(currentInteractable);
        if (currentInteractable == null)
        {
            personajeActual.CambiarEstado(null);
            personaje = null;
            return;
        }
        if (currentCharacter)
        {
            personaje = currentCharacter;
            currentCharacter.MovementComponent.MaxSpeed = maxSpeed;
        }
        else
        {
            personajeActual.CambiarEstado(null);
        }
        StopPlayerActions();
        currentCharacter.MovementComponent.UpdateRotationON = false;
        currentCharacter.MovementComponent.UpdatePositionON = false;
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
        currentCharacter.MovementComponent.UpdateRotationON = true;
        currentCharacter.MovementComponent.UpdatePositionON = true;
        base.Salir();
    }

    public override void Interact()
    {
        personaje.CambiarEstado(null);
    }

    public override void Move(Vector2 input)
    {
        currentCharacter.MovementComponent.Direction = input;
    }

}
