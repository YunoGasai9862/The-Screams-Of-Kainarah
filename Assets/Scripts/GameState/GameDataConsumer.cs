using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts.GameState
{
    public class GameDataConsumer: BaseState<GameDataBundle>
    {
        private StateEvent GameDataStateEvent { get; set; }

        protected override async Task AddEvent()
        {
            GameDataStateEvent = await Helper.GetCustomEvent<StateEvent>();
        }

        protected override UnityEvent<GameDataBundle> GetEvent()
        {

            return GameDataStateEvent;
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
}
