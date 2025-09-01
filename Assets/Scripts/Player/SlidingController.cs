using CoreCode;
using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>, IObserver<AnimationDetails>, ISubject<IObserver<CharacterVelocity>>, IObserver<Player>
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

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState,bool>> _animationHandler;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Player Player { get; set; }

    private bool IS_SLIDING { get; set; } = false;

    private void Awake()
    {
        PlayerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();

        AnimationDetailsDelegator = Helper.GetDelegator<AnimationDetailsDelegator>();

        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();
    }
void Start()
    {
        PlayerVelocityDelegator.AddToSubjectsDict(typeof(SlidingController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());
        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(SlidingController).ToString())[name].SetSubject(this);

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

        _animationHandler = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>(_animationHandler);

        _movementHelperClass = new MovementHelperClass();
    }

    private async Task Slide()
    {
        if (IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_SLIDING, IsConcluded = false };

            PlayerVelocityDelegator.NotifyObservers(new CharacterVelocity() { VelocityX = slidingSpeed }, gameObject.name, typeof(SlidingController), CancellationToken.None);

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>() { ExecutionState = PlayerAnimationExecutionState.PLAY_SLIDING_ANIMATION, Value = true });
        }

        if (AnimationDetails.CurrentAnimationTime > MAX_ANIMATION_TIME && AnimationDetails.CurrentAnimationStateInfo.IsName(PlayerAnimationConstants.SLIDING))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_SLIDING, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>() { ExecutionState = PlayerAnimationExecutionState.PLAY_SLIDING_ANIMATION, Value = false });
        }

    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        if (Player == null)
        {
            Debug.Log($"Player is null - SlidingController - PerformAction - Skipping execution");

            return await Task.FromResult(false);
        }

        if (await IsVelocityXGreaterThanZero(Player.Rigidbody) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            await Slide();
        }

        return await Task.FromResult(true);
    }
    async Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_SLIDING, IsConcluded = true };

        PlayerVelocityDelegator.NotifyObservers(new CharacterVelocity() { VelocityX = 0 }, gameObject.name, typeof(SlidingController), CancellationToken.None);

        await PlayerStateEvent.Invoke(PlayerStateBundle);

        return await Task.FromResult(true);
    }

    private Task<bool> IsVelocityXGreaterThanZero(Rigidbody2D rb)
    {
        return Task.FromResult(Mathf.Abs(rb.linearVelocity.x) > 0);
    }

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(SlidingController).ToString())[gameObject.name], observer);
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
