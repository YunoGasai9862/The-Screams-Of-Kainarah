
using Assets.Annotations;

[Subject(SubjectType = typeof(EmitAttackAnimationStateConsumer), ContextType = typeof(GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>))]
public class EmitAttackAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, AttackState>
{
    protected override GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                CurrentAnimation = new EmitAnimationStateBundle<bool>.CurrentAnimationInfo<bool>()
                {
                    CurrentValue = false,
                }
            }
        };
    }
}