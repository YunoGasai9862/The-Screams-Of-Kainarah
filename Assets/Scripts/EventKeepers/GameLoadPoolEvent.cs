using System.Threading.Tasks;
using UnityEngine.Events;
public class GameLoadPoolEvent : UnityEventWTAsync
{
    private UnityEvent m_gameLoadPoolEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_gameLoadPoolEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        m_gameLoadPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        m_gameLoadPoolEvent.Invoke();

        return Task.CompletedTask;
    }
}