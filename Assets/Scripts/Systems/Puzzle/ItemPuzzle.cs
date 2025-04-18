using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem
{
    public class ItemPuzzle : PuzzleInteractable
    {
        [SerializeField] InventorySystem.ItemSO itemNeeded;
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
            base.Interact();
            if (Completed || (!InventorySystem.Inventory.ContainsItem(itemNeeded) && itemNeeded != null))
            {
                return;
            }
            if (consumeObject && itemNeeded != null)
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
    }
}