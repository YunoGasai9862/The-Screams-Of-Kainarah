using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameStateConsumer : BaseState<GameStateBundle>
{
     private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; }

     private GameStateEvent GameStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        GlobalGameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        GameStateEvent = await Helper.GetCustomEvent<GameStateEvent>();
    }

    protected override async Task AddSubject()
    {
        GlobalGameStateDelegator.AddToSubjectsDict(typeof(GameStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<GameStateBundle>>>());

        GlobalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GameStateConsumer).ToString())[gameObject.name].SetSubject(this);

        Debug.Log($"Added to the dictionary for GameStateConsumer {GlobalGameStateDelegator.GetSubjectsDict().Count}");
    }

    protected override async Task<BaseDelegator<GenericStateBundle<GameStateBundle>>> GetDelegator()
    {
        return GlobalGameStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<GameStateBundle>>> GetEvent()
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
                    CurrentState = GameState.FREE_MOVEMENT
                }
            }
        };
    }
}