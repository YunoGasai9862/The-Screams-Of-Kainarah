

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class EntityPoolReadyEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_entityPoolReadyEvent = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return m_entityPoolReadyEvent;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        m_entityPoolReadyEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(bool value)
    {
        m_entityPoolReadyEvent.Invoke(value);

        return Task.CompletedTask;
    }
}