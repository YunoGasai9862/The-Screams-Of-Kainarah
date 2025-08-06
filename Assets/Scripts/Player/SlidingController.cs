using CoreCode;
using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>, IObserver<AnimationDetails>, ISubject<IObserver<CharacterVelocity>>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float slidingSpeed;

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    private AnimationDetailsDelegator AnimationDetailsDelegator { get; set;  }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private AnimationDetails AnimationDetails { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState,bool>> _animationHandler;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Collider2D _col;

    private Animator _anim;

    private Rigidbody2D _rb;

    private bool IS_SLIDING { get; set; } = false;

    private void Awake()
    {
        PlayerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();
    }
    void Start()
    {
        PlayerVelocityDelegator.AddToSubjectsDict(typeof(SlidingController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());
        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(SlidingController).ToString())[name].SetSubject(this);

        AnimationDetailsDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            SubjectType = typeof(PlayerAnimationController).ToString(),
        }, CancellationToken.None);

        _animationHandler = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>>();
        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>(_animationHandler);

        _movementHelperClass = new MovementHelperClass();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _col= GetComponent<CapsuleCollider2D>();
    }

    private async Task Slide()
    {
        if (IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
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
        if (await IsVelocityXGreaterThanZero(_rb) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
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
}
