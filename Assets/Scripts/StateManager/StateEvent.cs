using System.Threading.Tasks;
using UnityEngine.Events;

public abstract class StateEvent<T> : UnityEventWTAsync<SystemState<T>>
{
    private UnityEvent<SystemState<T>> m_stateEvent = new UnityEvent<SystemState<T>>();
    public override Task AddListener(UnityAction<SystemState<T>> action)
    {
        m_stateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<SystemState<T>> GetInstance()
    {
        return m_stateEvent;
    }

    public override Task Invoke(SystemState<T> value)
    {
        m_stateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}