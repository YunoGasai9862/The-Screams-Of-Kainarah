using System.Threading.Tasks;
using UnityEngine.Events;

public class GameStateEvent : UnityEventWTAsync<GameState>
{
    private UnityEvent<GameState> m_gameStateEvent = new UnityEvent<GameState>();
    public override Task AddListener(UnityAction<GameState> action)
    {
        m_gameStateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<GameState> GetInstance()
    {
        return m_gameStateEvent;
    }

    public override Task Invoke(GameState value)
    {
        m_gameStateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}