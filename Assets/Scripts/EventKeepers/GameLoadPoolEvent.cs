using System.Threading.Tasks;
using UnityEngine.Events;
public class GameLoadPoolEvent : UnityEventWTAsync<bool>
{
    private UnityEvent<bool> m_gameLoadPoolEvent = new UnityEvent<bool>();
    public override UnityEvent<bool> GetInstance()
    {
        return m_gameLoadPoolEvent;
    }
    public override Task AddListener(UnityAction<bool> action)
    {
        m_gameLoadPoolEvent.AddListener(action);

        return Task.CompletedTask;
    }
    public override Task Invoke(bool value)
    {
        m_gameLoadPoolEvent.Invoke(value);

        return Task.CompletedTask;
    }
}