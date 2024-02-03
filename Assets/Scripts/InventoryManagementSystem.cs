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

    public InventoryManagemenetEvent inventoryManagementEvent = new InventoryManagemenetEvent();

    public void Awake()
    {
        if(Instance == null)
            Instance = this;

       inventoryManagementEvent.AddListener(AddToInventory);
    }
    //create multiple events for removal and addition!
    public async void AddToInventory(bool value)
    {
        inventoryManagementEvent.ShouldAddToInventory = value;
        await AddEntryToInventory();
    }

    private Task AddEntryToInventory()
    {
        if(inventoryManagementEvent.ShouldAddToInventory)
        {

        }
        return Task.CompletedTask;
    }
    public GameObject GetSlotGameObjectIsAttachedTo(string tag)
    {
        return InventorySystem.GetSlotGameObjectIsAttachedTo(tag);
    }
}
