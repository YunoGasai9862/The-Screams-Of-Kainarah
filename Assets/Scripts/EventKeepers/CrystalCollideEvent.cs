using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;

public class CrystalCollideEvent : UnityEventWT<Collider2D, bool>
{
    private UnityEvent<Collider2D, bool> _crystalCollideEvent = new UnityEvent<Collider2D, bool>();
    public override Task AddListener(UnityAction<Collider2D, bool> action)
    {
        _crystalCollideEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override UnityEvent<Collider2D, bool> GetInstance()
    {
        return _crystalCollideEvent;
    }
    public override Task Invoke(Collider2D colliderValue, bool boolValue)
    {
        Debug.Log(colliderValue.gameObject.name);
        GetInstance().Invoke(colliderValue , boolValue);
        return Task.CompletedTask;
    }
}