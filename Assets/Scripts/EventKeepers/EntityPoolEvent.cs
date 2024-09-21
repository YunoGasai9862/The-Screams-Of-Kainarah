using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EntityPoolEvent : UnityEventWTAsync<EntityPool<GameObject>>
{
    private UnityEvent<EntityPool<GameObject>> m_objectPoolEvent = new UnityEvent<EntityPool<GameObject>>();    
    public override UnityEvent<EntityPool<GameObject>> GetInstance()
    {
        return m_objectPoolEvent;
    }
    public override Task AddListener(UnityAction<EntityPool<GameObject>> action)
    {
        m_objectPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(EntityPool<GameObject> value)
    {
        m_objectPoolEvent.Invoke(value);

        return Task.CompletedTask;
    }
}