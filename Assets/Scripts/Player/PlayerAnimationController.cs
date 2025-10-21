using PlayerAnimationHandler;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, ISubject<IObserver<AnimationDetails>>, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>, 
    IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<IEntityAnimator>, IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>>
{
    private AnimationStateMachine AnimationStateMachine { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>() { StateBundle = new PlayerStateBundle() };

    private GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState> EmitMovementAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, MovementState>() { StateBundle = new EmitAnimationStateBundle<bool>()};

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private AnimationDetailsDelegator AnimationDetailsDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private EmitAnimationMovementStateDelegator EmitAnimationMovementStateDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private Animator PlayerAnimator { get; set; }

    private async void Awake()
    {
        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        AnimationDetailsDelegator = await Helper.GetDelegator<AnimationDetailsDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        EmitAnimationMovementStateDelegator = await Helper.GetDelegator<EmitAnimationMovementStateDelegator>();

        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        if (PlayerStateDelegator == null)
        {
            throw new DelegatorNotFoundException("PlayerStateDelegator not found!!");
        }

        if (AnimationDetailsDelegator == null)
        {
            throw new DelegatorNotFoundException("AnimationDetailsDelegator not found!!");
        }

        if (PlayerStateEvent == null)
        {
            throw new CustomEventNotFoundException("PlayerStateEvent not found!!");
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

        StartCoroutine(PlayerStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None));

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

    public void MovementAnimation(bool keystroke)
    {
        if (EmitMovementAnimationStateBundle.StateBundle == null || EmitMovementAnimationStateBundle.StateBundle.Value)
        {
            Debug.Log($"Exiting because EmitMovementAnimationStateBundle.StateBundle.Value is: {EmitMovementAnimationStateBundle.StateBundle.Value}");
            return;
        }

        PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState>() { CurrentState = keystroke ?
            MovementState.IS_RUNNING : MovementState.IS_IDLE, IsConcluded = false };

        PlayerStateEvent.Invoke(PlayerStateBundle);

        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int)PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState);
    }

    private void JumpAnimation(bool keystroke)
    {
        PlayerStateBundle.StateBundle.PlayerMovementState = keystroke ?
            new State<MovementState>() { CurrentState = MovementState.IS_JUMPING, IsConcluded = false } : 
            new State<MovementState>() { CurrentState = MovementState.IS_FALLING, IsConcluded = false };

        PlayerStateEvent.Invoke(PlayerStateBundle);

        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int)PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState);
    }

    private void SlidingAnimation(bool keystroke)
    {
        PlayAnimation(PlayerAnimationConstants.SLIDING, keystroke);
    }

    private void PlayAnimation(string name, int state)
    {
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

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerStateBundle.StateBundle = data.StateBundle;
    }

    public Task<ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, bool>>> PerformAction(ControllerPackage<PlayerAnimationExecutionState, bool> value = null)
    {
        if (PlayerAnimator == null)
        {
            return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, bool>>(null));
        }

        GetAnimationExecutionScenario(value);

        return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, bool>>(value));
    }

    public Task<ActionExecuted<ControllerPackage<PlayerAnimationExecutionState, bool>>> CancelAction(ControllerPackage<PlayerAnimationExecutionState, bool> value = null)
    {
        throw new System.NotImplementedException();
    }

    public void GetAnimationExecutionScenario(ControllerPackage<PlayerAnimationExecutionState, bool> package)
    {
        switch(package.ExecutionState)
        {
            case PlayerAnimationExecutionState.PLAY_JUMPING_ANIMATION:
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
        Debug.Log($"Incoming Value: {data.StateBundle.Value}");

        EmitMovementAnimationStateBundle.StateBundle = data.StateBundle;
    }
}