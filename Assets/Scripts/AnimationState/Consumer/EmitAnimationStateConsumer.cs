using UnityEngine;
using UnityEngine.Events;

public class EmitAnimationStateConsumer : BaseState<EmitAnimationStateBundle>
{
    [SerializeField]
    EmitAnimationStateDelegator emitAnimationStateDelegator;

    [SerializeField]
    EmitAnimationStateEvent emitAnimationStateEvent;

    protected override void AddSubject()
    {
        emitAnimationStateDelegator.AddToSubjectsDict(typeof(EmitAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle>>>());

        emitAnimationStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAnimationStateConsumer).ToString())[name].SetSubject(this);
    }

    protected override BaseDelegatorEnhanced<GenericStateBundle<EmitAnimationStateBundle>> GetDelegator()
    {
        return emitAnimationStateDelegator;
    }

    protected override UnityEvent<GenericStateBundle<EmitAnimationStateBundle>> GetEvent()
    {
        return emitAnimationStateEvent.GetInstance();
    }

    protected override GenericStateBundle<EmitAnimationStateBundle> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle>()
        {
            StateBundle = new EmitAnimationStateBundle()
            {
                IsRunning = false
            }
        };
    }
}