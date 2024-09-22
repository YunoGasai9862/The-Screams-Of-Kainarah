using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EntityPoolEvent : UnityEventWTAsync<EntityPool<dynamic>>
{
    private UnityEvent<EntityPool<dynamic>> m_entityPoolEvent = new UnityEvent<EntityPool<dynamic>>();    
    public override UnityEvent<EntityPool<dynamic>> GetInstance()
    {
        return m_entityPoolEvent;
    }
    public override Task AddListener(UnityAction<EntityPool<dynamic>> action)
    {
        m_entityPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(EntityPool<dynamic> value)
    {
        m_entityPoolEvent.Invoke(value);

        return Task.CompletedTask;
    }
}