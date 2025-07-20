using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateConsumer : BaseState<GameStateBundle>
{
    [SerializeField] GlobalGameStateDelegator globalGameStateDelegator;

    [SerializeField] GameStateEvent gameStateEvent;

    protected override void AddSubject()
    {
        globalGameStateDelegator.AddToSubjectsDict(typeof(GameStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<GameStateBundle>>>());

        globalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GameStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override BaseDelegatorEnhanced<GenericStateBundle<GameStateBundle>> GetDelegator()
    {
        return globalGameStateDelegator;
    }

    protected override UnityEvent<GenericStateBundle<GameStateBundle>> GetEvent()
    {
        return gameStateEvent.GetInstance();
    }
}