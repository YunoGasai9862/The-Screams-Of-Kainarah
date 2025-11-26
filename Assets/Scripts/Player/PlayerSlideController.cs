using CoreCode;
using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSlideController : MonoBehaviour, IReceiverEnhancedAsync<PlayerSlideController, PlayerStateBundle>, IObserver<AnimationDetails>, ISubject<IObserver<CharacterVelocity>>, IObserver<Player>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float slidingSpeed;

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    private AnimationDetailsDelegator AnimationDetailsDelegator { get; set;  }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private AnimationDetails AnimationDetails { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>()
    { 
         StateBundle = new PlayerStateBundle()
    };

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationHandler;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Player Player { get; set; }

    private bool IS_SLIDING { get; set; } = false;

    private async void Awake()
    {
        PlayerVelocityDelegator = await Helper.GetDelegator<PlayerVelocityDelegator>();

        AnimationDetailsDelegator = await Helper.GetDelegator<AnimationDetailsDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>(); 
    }

    void Start()
    {
        PlayerVelocityDelegator.AddToSubjectsDict(typeof(PlayerSlideController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());
        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSlideController).ToString())[name].SetSubject(this);

        StartCoroutine(AnimationDetailsDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            SubjectType = typeof(PlayerAnimationController).ToString(),
        }, CancellationToken.None));

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            SubjectType = typeof(PlayerAttributesNotifier).ToString(),
        }, CancellationToken.None));

        _animationHandler = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationHandler);

        _movementHelperClass = new MovementHelperClass();
    }

    private async Task Slide()
    {
        if (IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, bool>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = true, IsConcluded = false };
            
            PlayerVelocityDelegator.NotifyObservers(new CharacterVelocity() { VelocityX = slidingSpeed }, gameObject.name, typeof(PlayerSlideController), CancellationToken.None);

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.SLIDING, Value = PlayerStateBundle.StateBundle });
        }

        if (AnimationDetails.CurrentAnimationTime > MAX_ANIMATION_TIME && AnimationDetails.CurrentAnimationStateInfo.IsName(PlayerAnimationConstants.SLIDING))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, bool>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = false, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.SLIDING, Value = PlayerStateBundle.StateBundle });
        }

    }

    public async Task<ActionExecuted> PerformAction(PlayerStateBundle value)
    {
        if (Player == null)
        {
            Debug.Log($"Player is null - SlidingController - PerformAction - Skipping execution");

            return await Task.FromResult(new ActionExecuted() { Result = false });
        }

        if (await IsVelocityXGreaterThanZero(Player.Rigidbody) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            await Slide();
        }

        return await Task.FromResult(new ActionExecuted() { Result = true });
    }
    public async Task<ActionExecuted> CancelAction(PlayerStateBundle value)
    {
        PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, bool>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = false, IsConcluded = true };

        PlayerVelocityDelegator.NotifyObservers(new CharacterVelocity() { VelocityX = 0 }, gameObject.name, typeof(PlayerSlideController), CancellationToken.None);

        await PlayerStateEvent.Invoke(PlayerStateBundle);

        return await Task.FromResult(new ActionExecuted() { Result = false });
    }

    private Task<bool> IsVelocityXGreaterThanZero(Rigidbody2D rb)
    {
        return Task.FromResult(Mathf.Abs(rb.linearVelocity.x) > 0);
    }

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSlideController).ToString())[gameObject.name], observer);
    }

    public void OnNotify(AnimationDetails data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AnimationDetails = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;

        _playerAttackStateMachine = new PlayerAttackStateMachine(data.Animator);
    }
}
