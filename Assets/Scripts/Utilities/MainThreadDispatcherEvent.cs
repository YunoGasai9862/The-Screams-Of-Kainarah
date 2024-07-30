using System;
using System.Threading.Tasks;
using UnityEngine.Events;

//to make it as generic as possible, we can type cast them in the listener methods/and check for anomalies if the class expecting the type is not being sent in as expected
public class MainThreadDispatcherEvent : UnityEventWTAsync<CustomActions>
{
    private UnityEvent<CustomActions> m_MainThreadDispatcherEvent = new UnityEvent<CustomActions>();

    public override Task AddListener(UnityAction<CustomActions> action)
    {
        m_MainThreadDispatcherEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<CustomActions> GetInstance()
    {
        return m_MainThreadDispatcherEvent;
    }

    public override Task Invoke(CustomActions value)
    {
        m_MainThreadDispatcherEvent.Invoke(value);

        return Task.CompletedTask;
    }
}