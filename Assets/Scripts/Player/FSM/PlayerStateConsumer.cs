using UnityEngine.Events;
using UnityEngine;

public class PlayerStateConsumer : BaseState<PlayerStateBundle>
{
    [SerializeField] PlayerStateDelegator playerStateDelegator;

    [SerializeField] PlayerStateEvent playerStateEvent;

    protected override void AddSubject()
    {
        playerStateDelegator.AddToSubjectsDict(typeof(PlayerStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<PlayerStateBundle>>>());

        playerStateDelegator.GetSubsetSubjectsDictionary(typeof(PlayerStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override BaseDelegatorEnhanced<GenericStateBundle<PlayerStateBundle>> GetDelegator()
    {
        return playerStateDelegator;
    }

    protected override UnityEvent<GenericStateBundle<PlayerStateBundle>> GetEvent()
    {
        return playerStateEvent.GetInstance();
    }
}