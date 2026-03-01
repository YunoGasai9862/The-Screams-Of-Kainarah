
public class EmitMovementAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, MovementState>
{
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