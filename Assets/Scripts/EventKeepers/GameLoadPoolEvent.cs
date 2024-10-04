using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
public class GameLoadPoolEvent : UnityEventWTAsync
{
    private UnityEvent m_gameLoadPoolEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_gameLoadPoolEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        Debug.Log("Adding Listener");

        m_gameLoadPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        m_gameLoadPoolEvent.Invoke();

        Debug.Log("Invoked");

        return Task.CompletedTask;
    }
}