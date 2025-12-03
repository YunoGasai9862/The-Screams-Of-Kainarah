using PlayerAnimationHandler;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAnimationResetController, State<AttackState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<MovementState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<ActionState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private async Task Reset<T>(State<T> state) where T: Enum
    {
        switch (state.Reset.state)
        {
            case ResetState.COMPLETE_RESET:
                AnimationStateMachine.ResetParameters();
                break;

            case ResetState.PARTIAL_RESET:
            case ResetState.REVERT:
                AnimationStateMachine.ResetParameters(state.Reset.resetParameters, state.Reset.state);
                break;
        }
    }

    public async Task<ActionExecuted> PerformAction(State<AttackState> value = null)
    {
        await Reset(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<AttackState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }

    public async Task<ActionExecuted> PerformAction(State<MovementState> value = null)
    {
        await Reset(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<MovementState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }

    public async Task<ActionExecuted> PerformAction(State<ActionState> value = null)
    {
        await Reset(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<ActionState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }
}