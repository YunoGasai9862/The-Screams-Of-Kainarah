using System;
using System.Collections.Generic;
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

public class StateEvent : Assets.Scripts.Actions.UnityAction
{
    private IDictionary<Type, Delegate> m_actionByType = new Dictionary<Type, Delegate>();

    public override void AddListener<T>(UnityAction<T> action)
    {
        if (!m_actionByType.ContainsKey(typeof(T)))
        {
            m_actionByType.Add(typeof(T), action);
        }
    }

    public override UnityAction<T> GetAction<T>()
    {
        if (m_actionByType.ContainsKey(typeof(T)))
        {
            return (UnityAction<T>) m_actionByType[typeof(T)];
        }

        return null;
    }

    public override void Invoke<T>(T value)
    {
        if (m_actionByType.ContainsKey(typeof(T)))
        {
            ((UnityAction<T>) m_actionByType[typeof(T)]).Invoke(value);
        }
    }
}