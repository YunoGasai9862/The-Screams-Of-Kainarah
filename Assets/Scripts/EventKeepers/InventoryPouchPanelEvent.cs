using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPouchPanelEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_inventoryPouchPanelEvent = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        m_inventoryPouchPanelEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<bool> GetInstance()
    {
        return m_inventoryPouchPanelEvent;
    }

    public override Task Invoke(bool value)
    {
        m_inventoryPouchPanelEvent.Invoke(value);

        return Task.CompletedTask;
    }

}
