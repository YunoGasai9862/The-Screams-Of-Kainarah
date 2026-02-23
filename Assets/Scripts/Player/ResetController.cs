using Assets.Annotations;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;

[Observer(ObserverType = typeof(ResetController), SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle))]
public class ResetController : MonoBehaviour, INotify<ResetBundle>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private Delegator Delegator { get; set; }

    private void Awake()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<ResetBundle>()
        {
            Instance = gameObject,
            SubjectType = typeof(ResetController)
        }, this));
    }

    private IEnumerator Reset(ResetSystem resetSystem)
    {
        if (AnimationStateMachine == null)
        {
            Debug.Log("AnimationStateMachine is null in Reset - exiting!");
            yield return null;
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

        yield return null;
    }

    public IEnumerator Notify(ResetBundle value)
    {
        yield return StartCoroutine(Reset(value.ResetSystem));
    }
}