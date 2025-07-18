using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public abstract class StateEvent<T> : UnityEventWTAsync<GenericStateBundle<T>> where T : IStateBundle
{
    private UnityEvent<GenericStateBundle<T>> m_stateEvent = new UnityEvent<GenericStateBundle<T>>();

    public override Task AddListener(UnityAction<GenericStateBundle<T>> action)
    {
        m_stateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<GenericStateBundle<T>> GetInstance()
    {
        return m_stateEvent;
    }

    public override Task Invoke(GenericStateBundle<T> value)
    {
        m_stateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}