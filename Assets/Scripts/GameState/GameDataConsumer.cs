namespace Assets.Scripts.GameState
{
    public class GameDataConsumer: BaseState<GameDataBundle>
    {
        protected override GenericStateBundle<GameDataBundle> GetInitialState()
        {
            return new GenericStateBundle<GameDataBundle>
            {
              StateBundle = new GameDataBundle
              {

              }
            };
        }
    }
}
