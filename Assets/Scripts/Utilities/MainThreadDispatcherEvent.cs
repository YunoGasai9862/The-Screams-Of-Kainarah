using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;

public class MainThreadDispatcherEvent : UnityEventWT<Action>
{
    private UnityEvent<Action> m_MainThreadDispatcherEvent = new UnityEvent<Action>();

    public override Task AddListener(UnityAction<Action> action)
    {
        m_MainThreadDispatcherEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<Action> GetInstance()
    {
        return m_MainThreadDispatcherEvent;
    }

    public override Task Invoke(Action value)
    {

        m_MainThreadDispatcherEvent.Invoke(value);

        return Task.CompletedTask;
    }
}