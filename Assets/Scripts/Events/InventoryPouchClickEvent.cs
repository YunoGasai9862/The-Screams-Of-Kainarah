using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPouchClickEvent : UnityEventWT<bool>
{
    public override Task AddListener(UnityAction<bool> action)
    {
        throw new System.NotImplementedException();
    }

    public override UnityEvent<bool> GetInstance()
    {
        throw new System.NotImplementedException();
    }
}
