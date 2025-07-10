using System.Threading.Tasks;
using UnityEngine.Events;

public class GameStateEvent : UnityEventWTAsync<GameStateConsumer>
{
    private UnityEvent<GameStateConsumer> m_gameStateEvent = new UnityEvent<GameStateConsumer>();
    public override Task AddListener(UnityAction<GameStateConsumer> action)
    {
        m_gameStateEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<GameStateConsumer> GetInstance()
    {
        return m_gameStateEvent;
    }

    public override Task Invoke(GameStateConsumer value)
    {
        m_gameStateEvent.Invoke(value);

        return Task.CompletedTask;
    }
}