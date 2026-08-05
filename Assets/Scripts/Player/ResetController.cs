using Assets.Annotations;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(ResetController), SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle))]
public class ResetController : MonoBehaviorScene, INotify<ResetBundle>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private Animator Animator { get; set; }

    private Delegator Delegator { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Start()
    {
        Animator = GetComponent<Animator>();

        SceneUtils = await (await GetBaseScene()).GetSceneUtilsAsync();

        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<ResetBundle>()
        {
            Instance = gameObject,
            EntityType = typeof(ResetController),
            SubjectType = typeof(PlayerAttackStateMachineReset)
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
                AnimationStateMachine.ResetParameters(Animator);
                break;

            case ResetState.PARTIAL_RESET:
            case ResetState.REVERT:
                AnimationStateMachine.ResetParameters(Animator, resetSystem.ResetParameters, resetSystem.State);
                break;
        }

        yield return null;
    }

    public IEnumerator Notify(ResetBundle value)
    {
        yield return StartCoroutine(Reset(value.ResetSystem));
    }
}