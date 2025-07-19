public class GameStateBundle: IStateBundle
{
    public GenericStateBundle<GameState> GameState { get; set; } = new GenericStateBundle<GameState>();
}