using Assets.Scripts.GenericDelegators;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ResetController : MonoBehaviour, ISubject<IObserver<ResetBundle>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private ResetControllerDelegator ResetControllerDelegator { get; set; }

    private async void Awake()
    {
        ResetControllerDelegator = await Helper.GetDelegator<ResetControllerDelegator>();

        ResetControllerDelegator.AddToSubjectsDict(tag, name, new Subject<IObserver<ResetBundle>>());

        ResetControllerDelegator.GetSubsetSubjectsDictionary(tag)[name].SetSubject(this);
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

    public void OnNotifySubject(IObserver<ResetBundle> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        
    }
}