using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAnimationResetController, ControllerPackage<Reset.ResetState, PlayerStateBundle>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private async Task ResetState(PlayerStateBundle data, Reset.ResetState reset)
    {
        switch (reset)
        {
            case Reset.ResetState.COMPLETE_RESET:
                AnimationStateMachine.ResetParameters();
                break;

            case Reset.ResetState.PARTIAL_RESET:
                AnimationStateMachine.ResetParameters(data.PlayerMovementState.Reset.ResetParameters);
                AnimationStateMachine.ResetParameters(data.PlayerActionState.Reset.ResetParameters);
                AnimationStateMachine.ResetParameters(data.PlayerAttackState.Reset.ResetParameters);
                break;

            case Reset.ResetState.REVERT:
                break;
        }
    }

    public async Task<ActionExecuted<ControllerPackage<Reset.ResetState, PlayerStateBundle>>> PerformAction(ControllerPackage<Reset.ResetState, PlayerStateBundle> value = null)
    {
        await ResetState(value.Value, value.ExecutionState);

        return new ActionExecuted<ControllerPackage<Reset.ResetState, PlayerStateBundle>>(value);
    }

    public Task<ActionExecuted<ControllerPackage<Reset.ResetState, PlayerStateBundle>>> CancelAction(ControllerPackage<Reset.ResetState, PlayerStateBundle> value = null)
    {
        throw new System.NotImplementedException();
    }
}