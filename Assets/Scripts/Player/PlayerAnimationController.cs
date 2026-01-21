using Assets.Annotations;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Models.Reset;
using PlayerAnimationHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

[Observer(SubjectType = typeof(EmitMovementAnimationStateConsumer), ObserverType = typeof(PlayerAnimationController), DataType = typeof(GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>))]
[Observer(SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(PlayerAnimationController), DataType = typeof(IEntityAnimator))]
[Subject(SubjectType = typeof(PlayerAnimationController), ContextType = typeof(AnimationDetails))]
public class PlayerAnimationController : MonoBehaviour, IRequest<AnimationDetails>, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>,
    INotify<IEntityAnimator>, INotify<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }
    private GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> EmitMovementAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
    { StateBundle = new EmitAnimationStateBundle<bool>() { PreviousAnimation = new EmitAnimationStateBundle<bool>.PreviousAnimationInfo() } };

    private Delegator Delegator { get; set; }

    private Animator PlayerAnimator { get; set; }

    private PlayerStateBundle InternalPlayerStateBundle { get; set; } = new PlayerStateBundle();
    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        if (Delegator == null)
        {
            throw new DelegatorNotFoundException("Delegator not found!!");
        }
    }

    private void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<IEntityAnimator>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerAnimationController),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>> ()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerAnimationController),
            SubjectType = typeof(EmitMovementAnimationStateConsumer)
        }, this));

    }

    public void MovementAnimation(PlayerStateBundle bundle, AnimationExecutionState executionState)
    {
        if (EmitMovementAnimationStateBundle.StateBundle == null || EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation == null)
        {
            Debug.Log($"Bundles are null - will skip Movement Animation!");
            return;
        }

        if (ShouldSkipMovementAnimation(bundle))
        {
            Debug.Log($"Values are the same - skipping! {InternalPlayerStateBundle.PlayerMovementState.CurrentState} - {bundle.PlayerMovementState.CurrentState}");
            return;
        }

        InternalPlayerStateBundle.PlayerMovementState = bundle.PlayerMovementState;

        EmitMovementAnimationStateBundle.StateBundle.PreviousAnimation.PreviousAnimationHash = EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation.CurrentAnimatorStateInfo.shortNameHash;

        AnimationStateMachine.SetAnimation(PlayerAnimationField.OverallState.ToString(), (int) executionState);
        AnimationStateMachine.SetAnimation(PlayerAnimationField.Speed.ToString(), bundle.PlayerMovementState.CurrentValue.CharacterSpeed.x);
    }

    private void JumpAnimation(PlayerStateBundle bundle, AnimationExecutionState executionState)
    {
        AnimationStateMachine.SetAnimation(PlayerAnimationField.OverallState.ToString(), (int) executionState);
        AnimationStateMachine.SetAnimation(PlayerAnimationField.LeapState.ToString(), (int) bundle.PlayerLeapState.CurrentState); 
    }

    private void SlidingAnimation(PlayerStateBundle bundle, AnimationExecutionState executionState)
    {
        AnimationStateMachine.SetAnimation(PlayerAnimationField.OverallState.ToString(), (int) executionState);
        AnimationStateMachine.SetAnimation(PlayerAnimationField.Sliding.ToString(), (int) bundle.PlayerMovementState.CurrentState);
    }

    private float ReturnCurrentAnimation()
    {
        return PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private AnimatorStateInfo GetCurrentStateInfo()
    {
        return PlayerAnimator.GetCurrentAnimatorStateInfo(0);
    }

    //check how emit movement animation state bundle is working - might need to fix it!
    private bool ShouldSkipMovementAnimation(PlayerStateBundle bundle)
    {
        return InternalPlayerStateBundle.PlayerMovementState != null && (int)InternalPlayerStateBundle.PlayerMovementState.CurrentState == (int)bundle.PlayerMovementState.CurrentState &&
               EmitMovementAnimationStateBundle.StateBundle.PreviousAnimation.PreviousAnimationHash != EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation.CurrentAnimatorStateInfo.shortNameHash;
    }

    public Task<ActionExecuted> PerformAction(ControllerPackage<AnimationExecutionState, PlayerStateBundle> value = null)
    {
        if (PlayerAnimator == null)
        {
            return Task.FromResult(new ActionExecuted() { Result = false });
        }

        GetAnimationExecutionScenario(value);

        return Task.FromResult(new ActionExecuted() { Result = true });
    }

    public void GetAnimationExecutionScenario(ControllerPackage<AnimationExecutionState, PlayerStateBundle> package)
    {
        switch(package.ExecutionState)
        {
            case AnimationExecutionState.LEAP:
                JumpAnimation(package.Value, package.ExecutionState);
                break;

            case AnimationExecutionState.INTERACTION:
                SlidingAnimation(package.Value, package.ExecutionState);
                break;

            case AnimationExecutionState.MOVEMENT:
                MovementAnimation(package.Value, package.ExecutionState);
                break;
            default:
                break;
        }
    }

    public Task<ActionExecuted> CancelAction(ControllerPackage<AnimationExecutionState, PlayerStateBundle> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = true });
    }

    public IEnumerator Request()
    {
        yield return new WaitUntil(() => PlayerAnimator != null);

        StartCoroutine(Delegator.NotifyObserver(new SubjectContext<AnimationDetails>() { Data = new AnimationDetails()
        {
            CurrentAnimationStateInfo = GetCurrentStateInfo(),
            CurrentAnimationTime = ReturnCurrentAnimation()
        }}, this));
    }

    public IEnumerator Notify(GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> value)
    {
        EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation = value.StateBundle.CurrentAnimation;

        yield return null;
    }

    public IEnumerator Notify(IEntityAnimator value)
    {
        PlayerAnimator = value.Animator;

        AnimationStateMachine = new AnimationStateMachine(PlayerAnimator);

        yield return null;
    }
}