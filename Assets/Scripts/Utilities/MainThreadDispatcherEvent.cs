using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;

public class MainThreadDispatcherEvent : UnityEventWT<Action, CancellationToken>
{
    private UnityEvent<Action, CancellationToken> m_MainThreadDispatcherEvent = new UnityEvent<Action, CancellationToken>();
    public override Task AddListener(UnityAction<Action, CancellationToken> action)
    {
        m_MainThreadDispatcherEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<Action, CancellationToken> GetInstance()
    {
        return m_MainThreadDispatcherEvent;
    }

    public override Task Invoke(Action tValue, CancellationToken zValue)
    {
        m_MainThreadDispatcherEvent?.Invoke(tValue, zValue);

        return Task.CompletedTask;
    }
}