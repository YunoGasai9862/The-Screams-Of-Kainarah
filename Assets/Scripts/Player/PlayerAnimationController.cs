using PlayerAnimationHandler;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, ISubject<IObserver<AnimationDetails>>, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>, 
    IObserver<IEntityAnimator>, IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }
    private GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> EmitMovementAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>()
    { StateBundle = new EmitAnimationStateBundle<bool>() { PreviousAnimation = new EmitAnimationStateBundle<bool>.PreviousAnimationInfo() } };

    private AnimationDetailsDelegator AnimationDetailsDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private EmitAnimationMovementStateDelegator EmitAnimationMovementStateDelegator { get; set; }

    private Animator PlayerAnimator { get; set; }

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
        AnimationDetailsDelegator.AddToSubjectsDict(typeof(PlayerAnimationController).ToString(), name, new Subject<IObserver<AnimationDetails>>());
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

    public void MovementAnimation(PlayerStateBundle bundle)
    {
        if (EmitMovementAnimationStateBundle.StateBundle == null || EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation == null)
        {
            Debug.Log($"Bundles are null - will skip Movement Animation!");
            return;
        }

        //need to stop the spam from 0 again!!
        EmitMovementAnimationStateBundle.StateBundle.PreviousAnimation.PreviousAnimationHash = EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation.CurrentAnimatorStateInfo.shortNameHash;

        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int)bundle.PlayerMovementState.CurrentState);
    }

    private void JumpAnimation(PlayerStateBundle bundle)
    {
        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int) bundle.PlayerMovementState.CurrentState); 
    }

    private void SlidingAnimation(PlayerStateBundle bundle)
    {
        PlayAnimation(PlayerAnimationConstants.SLIDING, bundle.PlayerMovementState.CurrentValue);
    }

    private void PlayAnimation(string name, int state)
    {
        Debug.Log($"NAME: {name}, state: {state}");
        AnimationStateMachine.AnimationPlayForInt(name, state);
    }
    private void PlayAnimation(string name, bool state)
    {
        AnimationStateMachine.AnimationPlayForBool(name, state);
    }

    private float ReturnCurrentAnimation()
    {
        return PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private AnimatorStateInfo GetCurrentStateInfo()
    {
        return PlayerAnimator.GetCurrentAnimatorStateInfo(0);
    }

    public Task<ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>> PerformAction(ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle> value = null)
    {
        if (PlayerAnimator == null)
        {
            return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>(null));
        }

        GetAnimationExecutionScenario(value);

        return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>(value));
    }

    public void GetAnimationExecutionScenario(ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle> package)
    {
        switch(package.ExecutionState)
        {
            case PlayerAnimationExecutionState.PLAY_IN_AIR_ANIMATION:
                JumpAnimation(package.Value);
                break;

            case PlayerAnimationExecutionState.PLAY_SLIDING_ANIMATION:
                SlidingAnimation(package.Value);
                break;

            case PlayerAnimationExecutionState.PLAY_MOVEMENT_ANIMATION:
                MovementAnimation(package.Value);
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
        Debug.Log($"Incoming Value: {data.StateBundle.CurrentAnimation.CurrentValue}");

        EmitMovementAnimationStateBundle.StateBundle.CurrentAnimation = data.StateBundle.CurrentAnimation;
    }

    public Task<ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle>>> CancelAction(ControllerPackage<PlayerAnimationExecutionState, PlayerStateBundle> value = null)
    {
        throw new System.NotImplementedException();
    }
}