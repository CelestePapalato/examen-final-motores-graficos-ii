using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item Data", menuName = "ScriptableObjects/Item Data", order = 1)]
    public class Item : ScriptableObject
    {
        [SerializeField] int _id;
        [SerializeField] string _name;
        [SerializeField] Sprite _itemSprite;
        public int ID { get { return _id; } }
        public string Name { get { return _name; } }
        public Sprite ItemSprite { get { return _itemSprite; } }
    }
}