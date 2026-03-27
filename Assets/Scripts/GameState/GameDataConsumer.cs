using Assets.Annotations;

namespace Assets.Scripts.GameState
{
    [Subject(SubjectType = typeof(GameDataConsumer), ContextType = typeof(GenericStateBundle<GameDataBundle>))]
    public class GameDataConsumer: BaseState<GameDataBundle>
    {
        protected override GenericStateBundle<GameDataBundle> GetInitialState()
        {
            return new GenericStateBundle<GameDataBundle>
            {
              StateBundle = new GameDataBundle
              {
                  //TODO idk?? lol
              }
            };
        }
    }
}
