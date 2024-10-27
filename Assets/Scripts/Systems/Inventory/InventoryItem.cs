using System;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Item _itemData;
        public void Grab()
        {
            Inventory.AddItem(_itemData);
            Destroy(gameObject);
        }
    }
}