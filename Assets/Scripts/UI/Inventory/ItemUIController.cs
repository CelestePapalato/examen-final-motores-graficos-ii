using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem {
    public class ItemUIController : MonoBehaviour
    {
        private ItemSO _itemData;
        public ItemSO ItemData
        {
            get { return _itemData; }
            set { _itemData = value;
                updateSprite();
            }
        }
        private Image _imageComponent;
        private void Awake()
        {
            _imageComponent = GetComponentInChildren<Image>();
        }

        private void updateSprite()
        {
            if(ItemData == null)
            {
                _imageComponent.sprite = null;
                return;
            }
            Sprite sprite = _itemData.ItemSprite;
            _imageComponent.sprite = sprite;
        }
    }
}