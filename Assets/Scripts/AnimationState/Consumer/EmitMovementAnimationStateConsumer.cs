using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitMovementAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, MovementState>
{
    private EmitAnimationMovementStateDelegator EmitAnimationMovementStateDelegator { get; set; }

    private EmitMovementAnimationStateEvent EmitMovementAnimationStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        EmitAnimationMovementStateDelegator = await Helper.GetDelegator<EmitAnimationMovementStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        EmitMovementAnimationStateEvent = await Helper.GetCustomEvent<EmitMovementAnimationStateEvent>();
    }

    protected override Task AddSubject()
    {
        EmitAnimationMovementStateDelegator.AddToSubjectsDict(typeof(EmitAttackAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>>());

        EmitAnimationMovementStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAttackAnimationStateConsumer).ToString())[name].SetSubject(this);

        return Task.CompletedTask;
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>> GetDelegator()
    { 
        return EmitAnimationMovementStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>> GetEvent()
    {
        return EmitMovementAnimationStateEvent.GetInstance();
    }

    protected override GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                Value = false
            }
        };
    }
}