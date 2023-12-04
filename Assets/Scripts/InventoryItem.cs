using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    private Sprite _sprite;
    private int _quantity;
    private string _tag;
    private GameObject _slotAttachedTo; //this has to be separated later on
    public InventoryItem(Sprite sprite, string tag)
    {
        GetItemSprite = sprite;
        GetTag = tag;
        GetQuantity = 1; //initial count
        GetSlotAttachedTo = null;
    }
    public Sprite GetItemSprite { get => _sprite; set => _sprite = value; }
    public int GetQuantity { get => _quantity; set => _quantity = value; }
    public string GetTag { get => _tag; set => _tag = value; }
    public GameObject GetSlotAttachedTo { get => _slotAttachedTo; set => _slotAttachedTo = value; }
    
}
