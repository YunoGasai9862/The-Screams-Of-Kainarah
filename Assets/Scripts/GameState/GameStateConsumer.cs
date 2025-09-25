using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameStateConsumer : BaseState<GameStateBundle>
{
     private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; }

     private GameStateEvent GameStateEvent { get; set; }

    protected override async void AddSubject()
    {
        GlobalGameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();

        GlobalGameStateDelegator.AddToSubjectsDict(typeof(GameStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<GameStateBundle>>>());

        GlobalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GameStateConsumer).ToString())[gameObject.name].SetSubject(this);

        Debug.Log($"Added to the dictionary for GameStateConsumer {GlobalGameStateDelegator.GetSubjectsDict().Count}");
    }

    protected override BaseDelegator<GenericStateBundle<GameStateBundle>> GetDelegator()
    {
        return GlobalGameStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<GameStateBundle>>> GetEvent()
    {
        GameStateEvent = await Helper.GetCustomEvent<GameStateEvent>();

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