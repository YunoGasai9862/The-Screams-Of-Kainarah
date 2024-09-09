using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class ObjectPoolActiveEvent: UnityEventWTAsync<ObjectPool>
{
    private UnityEvent<ObjectPool> m_objectPool = new UnityEvent<ObjectPool>();
    public override UnityEvent<ObjectPool> GetInstance()
    {
        return m_objectPool;
    }
    public override Task AddListener(UnityAction<ObjectPool> action)
    {
        m_objectPool.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(ObjectPool value)
    {
        m_objectPool.Invoke(value);

        return Task.CompletedTask;
    }
}