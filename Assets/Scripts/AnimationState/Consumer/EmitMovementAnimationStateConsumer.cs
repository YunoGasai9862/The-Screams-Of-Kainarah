using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitMovementAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, MovementState>
{
    private EmitMovementAnimationStateEvent EmitMovementAnimationStateEvent { get; set; }

    protected override async Task AddEvent()
    {
        EmitMovementAnimationStateEvent = await Helper.GetCustomEvent<EmitMovementAnimationStateEvent>();
    }

    protected override UnityEvent<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>> GetEvent()
    {
        return EmitMovementAnimationStateEvent.GetInstance();
    }

    protected override GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                CurrentAnimation = new EmitAnimationStateBundle<bool>.CurrentAnimationInfo<bool>()
                {
                    CurrentValue = true,
                }
            }
        };
    }
}