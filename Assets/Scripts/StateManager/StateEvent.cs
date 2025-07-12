using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public abstract class StateEvent<T> : UnityEventWTAsync<GenericState<T>> where T : Enum
{
    private UnityEvent<GenericState<T>> m_stateEvent = new UnityEvent<GenericState<T>>();

    public override Task AddListener(UnityAction<GenericState<T>> action)
    {
        m_stateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<GenericState<T>> GetInstance()
    {
        return m_stateEvent;
    }

    public override Task Invoke(GenericState<T> value)
    {
        m_stateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}