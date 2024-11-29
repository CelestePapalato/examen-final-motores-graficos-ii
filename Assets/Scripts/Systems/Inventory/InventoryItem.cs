using System;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryItem : Item
    {
        [SerializeField] private ItemSO _itemData;
        public override void Grab()
        {
            base.Grab();
            Inventory.AddItem(_itemData);
            Destroy(gameObject);
        }
    }
}