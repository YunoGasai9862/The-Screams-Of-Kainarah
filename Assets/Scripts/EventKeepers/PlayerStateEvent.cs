using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerStateEvent : UnityEventWTAsync<PlayerState>
{
    private UnityEvent<PlayerState> m_gameStateEvent = new UnityEvent<PlayerState>();
    public override Task AddListener(UnityAction<PlayerState> action)
    {
        m_gameStateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<PlayerState> GetInstance()
    {
        return m_gameStateEvent;
    }

    public override Task Invoke(PlayerState value)
    {
        m_gameStateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}