using Pathfinding;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EmitAnimationStateConsumer : BaseState<EmitAnimationStateBundle>
{
    private EmitAnimationStateDelegator EmitAnimationStateDelegator { get; set; }

    private EmitAnimationStateEvent EmitAnimationStateEvent { get; set; }

    protected override void AddSubject()
    {
        EmitAnimationStateDelegator.AddToSubjectsDict(typeof(EmitAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle>>>());

        EmitAnimationStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAnimationStateConsumer).ToString())[name].SetSubject(this);
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle>>> GetDelegator()
    { 
        EmitAnimationStateDelegator = await Helper.GetDelegator<EmitAnimationStateDelegator>();

        return EmitAnimationStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<EmitAnimationStateBundle>>> GetEvent()
    {
        EmitAnimationStateEvent = await Helper.GetCustomEvent<EmitAnimationStateEvent>();

        return EmitAnimationStateEvent.GetInstance();
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