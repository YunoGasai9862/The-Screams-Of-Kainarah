using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IObserver<GenericStateBundle<PlayerStateBundle>>
{
    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>> AnimationReceiver { get; set; }

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>> AnimationCommand { get; set; }

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private async void Awake()
    {
        AnimationReceiver = await Helper.FindReceiver<PlayerAnimationController, IReceiverBase<ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>>();

        AnimationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>(AnimationReceiver);

        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None);
    }

    private async Task ResetAnimation(PlayerStateBundle playerStateBundle)
    {
        await AnimationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>()
        {
            ExecutionState = PlayerAnimationExecutionState.RESET,
            Value = playerStateBundle
        });
    }

    private async Task ResetState(GenericStateBundle<PlayerStateBundle> data, Reset reset)
    {
        switch (reset.State)
        {
            case Reset.ResetState.COMPLETE_RESET:
                await ResetAnimation(data.StateBundle);
                break;

            case Reset.ResetState.REVERT:
                break;
        }
    }

    public async void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        await ResetState(data, data.StateBundle.PlayerActionState.Reset);
        await ResetState(data, data.StateBundle.PlayerMovementState.Reset);
        await ResetState(data, data.StateBundle.PlayerActionState.Reset);
    }
}