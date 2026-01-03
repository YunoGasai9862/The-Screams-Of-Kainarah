using Assets.Annotations;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[Observer(ObserverType = typeof(ResetController), SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle))]
public class ResetController : MonoBehaviour, INotify<ResetBundle>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private Delegator Delegator { get; set; }

    private async void Awake()
    {

    }

    private async Task Reset(ResetSystem resetSystem)
    {
        if (AnimationStateMachine == null)
        {
            Debug.Log("AnimationStateMachine is null in Reset - exiting!");
            return;
        }

        switch (resetSystem.State)
        {
            case ResetState.COMPLETE_RESET:
                AnimationStateMachine.ResetParameters();
                break;

            case ResetState.PARTIAL_RESET:
            case ResetState.REVERT:
                AnimationStateMachine.ResetParameters(resetSystem.ResetParameters, resetSystem.State);
                break;
        }
    }

    public async Task Notify(ResetBundle value)
    {
        await Reset(value.ResetSystem);
    }
}