using System;
using UnityEngine;
using UnityEngine.Events;


//notification should go to the base class i think for sure - otherwise overwrite
public class GameStateConsumer : BaseState<GameState>
{
    [SerializeField] GlobalGameStateDelegator globalGameStateDelegator;

    [SerializeField] GameStateEvent gameStateEvent;

    protected override void AddSubject()
    {
        globalGameStateDelegator.AddToSubjectsDict(typeof(GameStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericState<GameState>>>());

        globalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GameStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override BaseDelegatorEnhanced<GenericState<GameState>> GetDelegator()
    {
        return globalGameStateDelegator;
    }

    protected override UnityEvent<GenericState<GameState>> GetEvent()
    {
        return gameStateEvent.GetInstance();
    }
}