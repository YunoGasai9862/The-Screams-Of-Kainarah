using PlayerAnimationHandler;
using System;
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
                AnimationStateMachine.ResetParameters(state.Reset.ResetParameters);
                break;

            case Reset.ResetState.REVERT:
                break;
        }
    }

    public async Task<ActionExecuted<State<AttackState>>> PerformAction(State<AttackState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted<State<AttackState>>(value);
    }

    public Task<ActionExecuted<State<AttackState>>> CancelAction(State<AttackState> value = null)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ActionExecuted<State<MovementState>>> PerformAction(State<MovementState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted<State<MovementState>>(value);
    }

    public Task<ActionExecuted<State<MovementState>>> CancelAction(State<MovementState> value = null)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ActionExecuted<State<ActionState>>> PerformAction(State<ActionState> value = null)
    {
        await ResetState(value);

        return new ActionExecuted<State<ActionState>>(value);
    }

    public Task<ActionExecuted<State<ActionState>>> CancelAction(State<ActionState> value = null)
    {
        throw new System.NotImplementedException();
    }
}