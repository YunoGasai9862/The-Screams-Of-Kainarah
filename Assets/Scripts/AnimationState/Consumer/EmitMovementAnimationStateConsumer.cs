
using Annotations.Enums;
using Assets.Annotations;

[Subject(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(EmitMovementAnimationStateConsumer), ContextType = typeof(GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>))]
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