using UnityEngine.Events;
using UnityEngine;

public class PlayerStateConsumer : BaseState<PlayerState>
{
    [SerializeField] PlayerStateDelegator playerStateDelegator;

    [SerializeField] PlayerStateEvent playerStateEvent;

    protected override void AddSubject()
    {
        playerStateDelegator.AddToSubjectsDict(typeof(PlayerStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericState<PlayerState>>>());

        playerStateDelegator.GetSubsetSubjectsDictionary(typeof(PlayerStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override BaseDelegatorEnhanced<GenericState<PlayerState>> GetDelegator()
    {
        return playerStateDelegator;
    }

    protected override UnityEvent<GenericState<PlayerState>> GetEvent()
    {
        return playerStateEvent.GetInstance();
    }
}