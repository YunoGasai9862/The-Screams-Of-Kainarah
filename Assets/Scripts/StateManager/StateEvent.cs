using System.Threading.Tasks;
using UnityEngine;
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

public abstract class StateEvent<T, Z> : UnityEventWTAsync<GenericStateBundle<T, Z>> where T : IStateBundle
{
    private UnityEvent<GenericStateBundle<T, Z>> m_stateEvent = new UnityEvent<GenericStateBundle<T, Z>>();

    public override Task AddListener(UnityAction<GenericStateBundle<T, Z>> action)
    {
        m_stateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<GenericStateBundle<T, Z>> GetInstance()
    {
        return m_stateEvent;
    }

    public override Task Invoke(GenericStateBundle<T, Z> value)
    {
        m_stateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}

public abstract class StateEvent : UnityEventWT
{
    private UnityEvent m_stateEvent = new UnityEvent();

    public override void AddListener(UnityAction action)
    {
        m_stateEvent.AddListener(action);
    }

    public override UnityEvent GetInstance()
    {
        return m_stateEvent;
    }

    public override void Invoke(dynamic value)
    {
        m_stateEvent.Invoke();
    }
}