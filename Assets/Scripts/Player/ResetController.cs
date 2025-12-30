using Assets.Annotations;
using Assets.Scripts.GenericDelegators;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[Observer(ObserverType = typeof(ResetController), SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle)]
public class ResetController : MonoBehaviour, IObserver<ResetBundle>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private ResetBundleDelegator ResetBundleDelegator { get; set; }

    private async void Awake()
    {
        ResetBundleDelegator = await Helper.GetDelegator<ResetBundleDelegator>();

        ResetBundleDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttackStateMachineReset).ToString()
        }, CancellationToken.None);
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

    public async void OnNotify(ResetBundle data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        await Reset(data.ResetSystem);
    }
}