using PlayerAnimationHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAnimationResetController, State<AttackState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<MovementState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<ActionState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private async Task ResetState<T>(State<T> state) where T: Enum
    {
        switch (state.Reset.State)
        {
            case Reset.ResetState.COMPLETE_RESET:
                AnimationStateMachine.ResetParameters();
                break;

            case Reset.ResetState.PARTIAL_RESET:
            case Reset.ResetState.REVERT:
                AnimationStateMachine.ResetParameters(state.Reset.ResetParameters, state.Reset.State);
                break;
        }
    }

    public async Task<ActionExecuted> PerformAction(State<AttackState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<AttackState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }

    public async Task<ActionExecuted> PerformAction(State<MovementState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<MovementState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }

    public async Task<ActionExecuted> PerformAction(State<ActionState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted() { Result = true };
    }

    public Task<ActionExecuted> CancelAction(State<ActionState> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = false });
    }
}