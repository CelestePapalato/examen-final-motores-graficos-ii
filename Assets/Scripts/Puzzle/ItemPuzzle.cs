using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem
{
    public class ItemPuzzle : PuzzleInteractable
    {
        [SerializeField] InventorySystem.Item itemNeeded;
        [SerializeField] bool consumeObject = true;
        [SerializeField] 
        [Tooltip("Cuando el puzzle ha sido completado, se activa el objeto para que sea visible")]
        GameObject finishedState;
        [SerializeField] GameObject toDestroyWhenFinished;

        private void Awake()
        {
            if (finishedState)
            {
                finishedState.SetActive(false);
            }
        }

        public override void Interact()
        {
            if (!InventorySystem.Inventory.ContainsItem(itemNeeded) || Completed)
            {
                return;
            }
            if (consumeObject)
            {
                InventorySystem.Inventory.RemoveItem(itemNeeded);
            }
            Completed = true;
            if(finishedState)
            {
                finishedState.SetActive(true);
            }
            if (toDestroyWhenFinished)
            {
                Destroy(toDestroyWhenFinished);
            }
        }

        public override void StopInteraction() { }
    }
}