using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryManagementSystem : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;

    private static InventoryManagementSystem _instance;
    private InventorySystem InventorySystem { get => inventorySystem; }
    public static InventoryManagementSystem Instance { get => _instance; private set => _instance = value; }

    private AddInventoryItemEvent _addInventoryItemEvent = new();
    private RemoveInventoryItemEvent _removeInventoryItemEvent = new();
    private AddInventoryItemEvent AddInventoryItemEvent { get => _addInventoryItemEvent; set=>_addInventoryItemEvent = value; }
    private RemoveInventoryItemEvent RemoveInventoryItemEvent { get => _removeInventoryItemEvent; set => _removeInventoryItemEvent = value; }

    public void Awake()
    {
        if(Instance == null)
            Instance = this;

        AddInventoryItemEvent.AddListener(AddToInventory);
        RemoveInventoryItemEvent.AddListener(RemoveFromInventory);
    }
    private async void AddToInventory(Sprite sprite, string itemTag)
    {
        AddInventoryItemEvent.Sprite = sprite;
        AddInventoryItemEvent.Item = itemTag;
        if (AddInventoryItemEvent.Sprite != null && !string.IsNullOrEmpty(AddInventoryItemEvent.Item))
        {
            await InventorySystem.AddToInventorySystem(AddInventoryItemEvent.Sprite, AddInventoryItemEvent.Item);
        }
    }
    private async void RemoveFromInventory(string item)
    {
        RemoveInventoryItemEvent.Item = item;
        if(!string.IsNullOrEmpty(RemoveInventoryItemEvent.Item))
        {
            await InventorySystem.ReduceQuantity(RemoveInventoryItemEvent.Item);
        }
    }
    public Task<bool> DoesItemExistInInventory(string tag)
    {
        GameObject item = InventorySystem.GetSlotGameObjectIsAttachedTo(tag);
        return item == null ? Task.FromResult(false) : Task.FromResult(true);
    }
    public Task<string> GetItemTagFromInventoryToDecreaseFunds(string tag)
    {
        GameObject item = InventorySystem.GetSlotGameObjectIsAttachedTo(tag);
        return item == null ? Task.FromResult(String.Empty) :Task.FromResult(item.transform.parent.gameObject.tag);
    }

    public void AddToSlot(GameObject entity)
    {
        InventorySystem.InventorySlots.Add(entity);
    }

    public void AddInvoke(Sprite sprite, string itemTag)
    {
        AddInventoryItemEvent.Invoke(sprite, itemTag);
    }
    public void RemoveInvoke(string itemTag)
    {
        RemoveInventoryItemEvent.Invoke(itemTag);
    }
}
