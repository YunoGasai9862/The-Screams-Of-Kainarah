using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class EntityPoolManagerActiveEvent: UnityEventWTAsync<EntityPoolManager>
{
    private UnityEvent<EntityPoolManager> m_objectPool = new UnityEvent<EntityPoolManager>();
    public override UnityEvent<EntityPoolManager> GetInstance()
    {
        return m_objectPool;
    }
    public override Task AddListener(UnityAction<EntityPoolManager> action)
    {
        m_objectPool.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(EntityPoolManager value)
    {
        m_objectPool.Invoke(value);

        return Task.CompletedTask;
    }
}