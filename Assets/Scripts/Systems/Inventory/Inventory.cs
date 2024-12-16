using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public static class Inventory
    {
        private static List<ItemSO> _currentItems = new List<ItemSO>();
        public static ItemSO[] CurrentItems { get { return _currentItems.ToArray(); } }

        public static UnityAction<ItemSO[]> InventoryUpdated;
        private static void _inventoryUpdated()
        {
            if(InventoryUpdated == null)
            {
                return;
            }
            ItemSO[] inventoryCopy = new ItemSO[_currentItems.Count];
            _currentItems.CopyTo(inventoryCopy, 0);
            InventoryUpdated?.Invoke(inventoryCopy);
        }

        public static void AddItem(ItemSO itemToAdd)
        {
            _currentItems.Add(itemToAdd);
            _inventoryUpdated();
        }

        public static void RemoveItem(ItemSO itemToRemove)
        {
            if (!_currentItems.Contains(itemToRemove))
            {
                Debug.Log(itemToRemove.Name + " is not contained in Inventory");
                return;
            }
            _currentItems.Remove(itemToRemove);
            _inventoryUpdated();
        }

        public static bool ContainsItem(ItemSO itemToCheck)
        {
            return _currentItems.Contains(itemToCheck);
        }

        public static void Clear()
        {
            _currentItems = new List<ItemSO>();
            ItemSO[] inventoryCopy = new ItemSO[_currentItems.Count];
            _currentItems.CopyTo(inventoryCopy, 0);
            InventoryUpdated?.Invoke(inventoryCopy);
        }
    }
}