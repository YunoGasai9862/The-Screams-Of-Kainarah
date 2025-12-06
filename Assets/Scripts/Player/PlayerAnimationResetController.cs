using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IObserver<Player>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<AttackState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<MovementState>>, IReceiverEnhancedAsync<PlayerAnimationResetController, State<ActionState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private Player Player { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }


    private async void Awake()
    {
        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None);
    }

    private async Task Reset<T>(State<T> state) where T: Enum
    {
        if (AnimationStateMachine == null)
        {
            Debug.Log("AnimationStateMachine is null in Reset - exiting!");
            return;
        }

        switch (state.Reset.State)
        {
            case ResetState.COMPLETE_RESET:
                AnimationStateMachine.ResetParameters();
                break;

            case ResetState.PARTIAL_RESET:
            case ResetState.REVERT:
                AnimationStateMachine.ResetParameters(state.Reset.ResetParameters, state.Reset.State);
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

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;

        AnimationStateMachine = new AnimationStateMachine(Player.Animator);
    }
}