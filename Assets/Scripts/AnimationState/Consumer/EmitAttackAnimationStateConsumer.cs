using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitAttackAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, AttackState>
{
    private EmitAttackAnimationStateEvent EmitAnimationStateEvent { get; set; }

    protected override async Task AddEvent()
    {
        EmitAnimationStateEvent = await Helper.GetCustomEvent<EmitAttackAnimationStateEvent>();
    }

    protected override UnityEvent<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>> GetEvent()
    {
        return EmitAnimationStateEvent.GetInstance();
    }

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