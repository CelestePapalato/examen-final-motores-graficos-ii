using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public static class Inventory
    {
        private static List<Item> _currentItems = new List<Item>();
        public static Item[] CurrentItems { get { return _currentItems.ToArray(); } }

        public static UnityAction<Item[]> InventoryUpdated;
        private static void _inventoryUpdated()
        {
            if(InventoryUpdated == null)
            {
                return;
            }
            Item[] inventoryCopy = new Item[_currentItems.Count];
            _currentItems.CopyTo(inventoryCopy, 0);
            InventoryUpdated(inventoryCopy);
        }

        public static void AddItem(Item itemToAdd)
        {
            _currentItems.Add(itemToAdd);
            _inventoryUpdated();
        }

        public static void RemoveItem(Item itemToRemove)
        {
            if (!_currentItems.Contains(itemToRemove))
            {
                Debug.Log(itemToRemove.Name + " is not contained in Inventory");
                return;
            }
            _currentItems.Remove(itemToRemove);
            _inventoryUpdated();
        }

        public static bool ContainsItem(Item itemToCheck)
        {
            return _currentItems.Contains(itemToCheck);
        }
    }
}