using System.Threading.Tasks;
using UnityEngine.Events;

public class ObjectPoolEvent : UnityEventWTAsync<EntityPool>
{
    private UnityEvent<EntityPool> m_objectPoolEvent = new UnityEvent<EntityPool>();    
    public override UnityEvent<EntityPool> GetInstance()
    {
        return m_objectPoolEvent;
    }
    public override Task AddListener(UnityAction<EntityPool> action)
    {
        m_objectPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(EntityPool value)
    {
        m_objectPoolEvent.Invoke(value);

        return Task.CompletedTask;
    }
}