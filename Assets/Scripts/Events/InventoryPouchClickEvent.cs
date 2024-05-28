using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPouchClickEvent : UnityEventWT<bool>
{
    private UnityEvent<bool> m_inventoryClickEvent = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        m_inventoryClickEvent.AddListener(action);  
        return Task.CompletedTask;
    }

    public override UnityEvent<bool> GetInstance()
    {
        return m_inventoryClickEvent;
    }
}
