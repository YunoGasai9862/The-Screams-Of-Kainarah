using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    private GameObject _item;
    private int _quantity;
    public InventoryItem(GameObject gameObject)
    {
        GetInventoryItem = gameObject;
        GetQuantity = 1; //initial count
    }
    public GameObject GetInventoryItem { get => _item; set => _item = value; }
    public int GetQuantity { get => _quantity; set => _quantity = value; }
    
}
