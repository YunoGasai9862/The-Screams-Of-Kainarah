using PlayerAnimationHandler;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, ISubject<AnimationDetails>, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>, 
    IObserver<IEntityAnimator>, IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }
    private GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> EmitMovementAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
    { StateBundle = new EmitAnimationStateBundle<bool>() { PreviousAnimation = new EmitAnimationStateBundle<bool>.PreviousAnimationInfo() } };

    private AnimationDetailsDelegator AnimationDetailsDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private EmitAnimationMovementStateDelegator EmitAnimationMovementStateDelegator { get; set; }

    private Animator PlayerAnimator { get; set; }

    private PlayerStateBundle InternalPlayerStateBundle { get; set; } = new PlayerStateBundle();
    private async void Awake()
    {
        AnimationDetailsDelegator = await Helper.GetDelegator<AnimationDetailsDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        EmitAnimationMovementStateDelegator = await Helper.GetDelegator<EmitAnimationMovementStateDelegator>();

        if (AnimationDetailsDelegator == null)
        {
            throw new DelegatorNotFoundException("AnimationDetailsDelegator not found!!");
        }

        if (PlayerAttributesDelegator == null)
        {
            throw new DelegatorNotFoundException("PlayerAttributesDelegator not found!!");
        }
    }

    private void Start()
    {
        AnimationDetailsDelegator.AddToSubjectsDict(typeof(PlayerAnimationController).ToString(), name, new Subject<AnimationDetails>());
        AnimationDetailsDelegator.GetSubsetSubjectsDictionary(typeof(PlayerAnimationController).ToString())[name].SetSubject(this);

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));

        StartCoroutine(EmitAnimationMovementStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(EmitMovementAnimationStateConsumer).ToString()
        }, CancellationToken.None));

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

    private IEnumerator NotifyAnimationDetailsObservers(IObserver<AnimationDetails> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        yield return new WaitUntil(() => PlayerAnimator != null);

        StartCoroutine(AnimationDetailsDelegator.NotifyObserver(observer, new AnimationDetails()
        {
            CurrentAnimationStateInfo = GetCurrentStateInfo(),
            CurrentAnimationTime = ReturnCurrentAnimation()
        },
        new NotificationContext()
        {
            SubjectType = typeof(PlayerAnimationController).ToString()
        },
        CancellationToken.None));
    }


    public void OnNotifySubject(IObserver<AnimationDetails> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(NotifyAnimationDetailsObservers(observer, notificationContext, cancellationToken, semaphoreSlim, optional));
    }

    public void OnNotify(IEntityAnimator data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerAnimator = data.Animator;

        AnimationStateMachine = new AnimationStateMachine(PlayerAnimator);
    }

    public void OnNotify(GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation = data.StateBundle.CurrentAnimation;
    }

    public Task<ActionExecuted> CancelAction(ControllerPackage<AnimationExecutionState, PlayerStateBundle> value = null)
    {
        return Task.FromResult(new ActionExecuted() { Result = true });
    }
}