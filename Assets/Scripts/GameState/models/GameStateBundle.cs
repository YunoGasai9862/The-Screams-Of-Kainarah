public class GameStateBundle: IStateBundle
{
    public State<GameState> GameState { get; set; } = new State<GameState>();
}