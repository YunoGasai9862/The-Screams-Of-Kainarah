using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;

namespace Assets.Scripts.GameState
{
    public class GameDataConsumer: BaseState<GameDataBundle>
    {
        //UnityEventType -> check if oyu can utilize that
        private GameDataEvent GameDataEvent { get; set; }

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
}
