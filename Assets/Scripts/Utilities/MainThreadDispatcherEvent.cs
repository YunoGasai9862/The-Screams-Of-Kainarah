using System;
using System.Threading.Tasks;
using UnityEngine.Events;

//to make it as generic as possible, we can type cast them in the listener methods/and check for anomalies if the class expecting the type is not being sent in as expected
public class MainThreadDispatcherEvent : UnityEventWTAsync<Action<object>, object>
{
    private UnityEvent<Action<object>, object> m_MainThreadDispatcherEvent = new UnityEvent<Action<object>, object>();

    public override Task AddListener(UnityAction<Action<object>, object> action)
    {
        m_MainThreadDispatcherEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<Action<object>, object> GetInstance()
    {
        return m_MainThreadDispatcherEvent;
    }

    public override Task Invoke(Action<object> value, object parameter)
    {

        m_MainThreadDispatcherEvent.Invoke(value, parameter);

        return Task.CompletedTask;
    }
}