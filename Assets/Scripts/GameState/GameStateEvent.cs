using UnityEngine.Events;

public class GameStateEvent : StateEvent<GameState>
{
    public UnityEvent<GenericState<GameState>> Event { get => GetInstance(); }
}