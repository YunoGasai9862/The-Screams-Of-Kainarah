using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class SceneSingletonActiveEvent : UnityEventWTAsync
{
    private UnityEvent m_sceneSingletonActiveEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_sceneSingletonActiveEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        m_sceneSingletonActiveEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        m_sceneSingletonActiveEvent.Invoke();

        return Task.CompletedTask;
    }
}