using UnityEngine.Events;
using UnityEngine;

public class PlayerStateConsumer : BaseState<PlayerState>
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
        return gameStateEvent.Event;
    }
}