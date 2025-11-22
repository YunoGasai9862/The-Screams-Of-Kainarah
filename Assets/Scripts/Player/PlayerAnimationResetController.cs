using PlayerAnimationHandler;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationResetController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAnimationResetController, ControllerPackage<Reset.ResetState, PlayerStateBundle>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private async Task ResetAnimation(PlayerStateBundle playerStateBundle)
    {
        AnimationStateMachine.ResetParameters();
    }

    private async Task ResetState(GenericStateBundle<PlayerStateBundle> data, Reset reset)
    {
        switch (reset.State)
        {
            case Reset.ResetState.COMPLETE_RESET:
                await ResetAnimation(data.StateBundle);
                break;

            case Reset.ResetState.PARTIAL_RESET:
                break;

            case Reset.ResetState.REVERT:
                break;
        }
    }

    //use the controller/command etc from the animation reset script!

    public Task<ActionExecuted<ControllerPackage<Reset.ResetState, PlayerStateBundle>>> PerformAction(ControllerPackage<Reset.ResetState, PlayerStateBundle> value = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<ActionExecuted<ControllerPackage<Reset.ResetState, PlayerStateBundle>>> CancelAction(ControllerPackage<Reset.ResetState, PlayerStateBundle> value = null)
    {
        throw new System.NotImplementedException();
    }
}