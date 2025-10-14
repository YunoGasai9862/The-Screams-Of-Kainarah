using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitAnimationStateConsumer : BaseState<EmitAnimationStateBundle>
{
    private EmitAnimationAttackStateDelegator EmitAnimationAttackStateDelegator { get; set; }

    private EmitAnimationStateEvent EmitAnimationStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        EmitAnimationAttackStateDelegator = await Helper.GetDelegator<EmitAnimationAttackStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        EmitAnimationStateEvent = await Helper.GetCustomEvent<EmitAnimationStateEvent>();
    }

    protected override Task AddSubject()
    {
        EmitAnimationAttackStateDelegator.AddToSubjectsDict(typeof(EmitAnimationStateConsumer).ToString(), name, new Subject<IObserver<AnimationStateBundle<EmitAnimationStateBundle, IAnimationState<PlayerAttackState>>>>());

        EmitAnimationAttackStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAnimationStateConsumer).ToString())[name].SetSubject(this);

        return Task.CompletedTask;
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle>>> GetDelegator()
    { 
        return  EmitAnimationAttackStateDelegator;
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