using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EntityPoolEvent : UnityEventWTAsync<AbstractEntityPool>
{
    private UnityEvent<AbstractEntityPool> m_entityPoolEvent = new UnityEvent<AbstractEntityPool>();    
    public override UnityEvent<AbstractEntityPool> GetInstance()
    {
        return m_entityPoolEvent;
    }
    public override Task AddListener(UnityAction<AbstractEntityPool> action)
    {
        m_entityPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(AbstractEntityPool value)
    {
        Debug.Log(value.ToString());
        m_entityPoolEvent.Invoke(value);

        return Task.CompletedTask;
    }
}