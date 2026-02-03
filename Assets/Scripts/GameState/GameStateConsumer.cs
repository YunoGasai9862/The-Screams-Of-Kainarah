using System.Threading.Tasks;
using UnityEngine.Events;

public class GameStateConsumer : BaseState<GameStateBundle>
{
     private GameStateEvent GameStateEvent { get; set; }

    protected override async Task AddEvent()
    {
        GameStateEvent = await Helper.GetCustomEvent<GameStateEvent>();
    }

    protected override UnityEvent<GenericStateBundle<GameStateBundle>> GetEvent()
    {
        return GameStateEvent.GetInstance();
    }

    protected override GenericStateBundle<GameStateBundle> GetInitialState()
    {
        return new GenericStateBundle<GameStateBundle>
        {
            StateBundle = new GameStateBundle()
            {
                GameState = new State<GameState>()
                {
                    CurrentState = GameState.FREE_MOVEMENT,

                    IsConcluded = false
                }
            }
        };
    }
}