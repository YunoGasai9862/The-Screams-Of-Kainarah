
using Assets.Annotations;

[Subject(SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
public class GameStateConsumer : BaseState<GameStateBundle>
{
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