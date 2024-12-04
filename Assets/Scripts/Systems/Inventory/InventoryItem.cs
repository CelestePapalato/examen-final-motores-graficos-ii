using System;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryItem : Item
    {
        [SerializeField] private ItemSO _itemData;
        [SerializeField] private GameObject destroyOnPick;
        public override void Grab()
        {
            base.Grab();
            Inventory.AddItem(_itemData);
            if (!destroyOnPick) { destroyOnPick = gameObject; }
            Destroy(destroyOnPick);
        }
    }
}