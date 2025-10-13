using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitAnimationStateConsumer : BaseState<EmitAnimationStateBundle>
{
    private EmitAnimationStateDelegator EmitAnimationStateDelegator { get; set; }

    private EmitAnimationStateEvent EmitAnimationStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        EmitAnimationStateDelegator = await Helper.GetDelegator<EmitAnimationStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        EmitAnimationStateEvent = await Helper.GetCustomEvent<EmitAnimationStateEvent>();
    }

    protected override Task AddSubject()
    {
        EmitAnimationStateDelegator.AddToSubjectsDict(typeof(EmitAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle>>>());

        EmitAnimationStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAnimationStateConsumer).ToString())[name].SetSubject(this);

        return Task.CompletedTask;
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle>>> GetDelegator()
    { 
        return EmitAnimationStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<EmitAnimationStateBundle>>> GetEvent()
    {
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