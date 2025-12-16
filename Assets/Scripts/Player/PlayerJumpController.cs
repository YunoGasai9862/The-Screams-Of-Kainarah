using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour, IReceiverEnhancedAsync<PlayerJumpController, bool>, ISubject<IObserver<CharacterVelocity>>, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<Player>
{
    [SerializeField] LayerMask groundLayer;

    [SerializeField] LayerMask ledgeLayer;

    [SerializeField] float JumpSpeed;

    [SerializeField] float maxJumpHeight;

    private const float FALLING_SPPED_RATIO = 0.8f;

    private const float JUMPING_SPEED_RATIO = 0.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    
    private const float MAX_JUMP_TIME = 0.3f;

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private bool _isJumpPressed;

    private Vector3 _playerInitialPosition;

    private CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity();

    private Player Player { get; set; }

    public Vector3 PlayerInitialPosition { get => _playerInitialPosition; set=> _playerInitialPosition = value; }

    public float TimeEclipsed { get; set; }

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle> { StateBundle = new PlayerStateBundle() };

    private async void Awake()
    {
        _movementHelperClass = new MovementHelperClass();

        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        PlayerVelocityDelegator = await Helper.GetDelegator<PlayerVelocityDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        _animationReceiver = await Helper.FindReceiver<PlayerAnimationController, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationReceiver);
    }
    private void Start()
    {
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

        PlayerVelocityDelegator.AddToSubjectsDict(typeof(PlayerJumpController).ToString(), gameObject.name, new Subject<IObserver<CharacterVelocity>>());

        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(PlayerJumpController).ToString())[gameObject.name].SetSubject(this);
    }

    //MORE CLEANUP
    public async Task HandleFalling()
    {
        if (await CanPlayerFall(maxJumpHeight) || !_isJumpPressed )
        {
            Debug.Log($"HandleFalling - CanPlayerFall");

            CharacterVelocity.VelocityY = (-1)  * JumpSpeed * FALLING_SPPED_RATIO;

            PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(PlayerJumpController), CancellationToken.None);
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && IsYVelocityNegative(CharacterVelocity))
        {
            Debug.Log($"HandleFalling - IsYVelocityNegative");

            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_FALLING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle });
        }

        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed) 
        {
            Debug.Log($"HandleFalling - (IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed)");

            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IDLE, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle });

            CharacterVelocity.VelocityY = 0;

            PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(PlayerJumpController), CancellationToken.None);

            TimeEclipsed = 0;
        }

        await Task.FromResult(true);
    }

    //MORE CLEANUP
    public async Task HandleJumping()
    {
        //add max height check
        if (CanPlayerJump()) 
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_JUMPING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED_RATIO;

            PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(PlayerJumpController), CancellationToken.None);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle});
        }
    }

    private bool CanPlayerJump()
    {
        return !PlayerStateBundle.StateBundle.PlayerLeapState.CurrentState.Equals(LeapState.IS_JUMPING) && (IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && _isJumpPressed && CharacterVelocity.VelocityY == 0f;
    }

    private Task SetPlayerInitialPosition(State<MovementState, MovementDto> currentPlayerState)
    {
        if((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !currentPlayerState.CurrentState.Equals(LeapState.IS_JUMPING))
        {
            PlayerInitialPosition = Player.Transform.position;
        }

        return Task.CompletedTask;
    }
    private async Task<bool> CanPlayerFall(float maxJumpHeight)
    {
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) && IsOnTheLedge(ledgeLayer));
        return MovementHelperFunctions.boolConditionAndTester(!isOnLedgeOrGround, MaxJumpHeightChecker(maxJumpHeight));
    }
    private bool IsOnTheGround(LayerMask ground)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    private bool IsOnTheLedge(LayerMask ledge)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    public bool MaxJumpHeightChecker(float maxJumpHeight)
    {
        return Player.Transform.position.y >= PlayerInitialPosition.y + maxJumpHeight;
    }

    private bool IsYVelocityNegative(CharacterVelocity characterVelocity)
    {
        return characterVelocity.VelocityY < 0 ? true : false;
    }

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(PlayerJumpController).ToString())[gameObject.name], observer);

        StartCoroutine(PlayerVelocityDelegator.NotifyObserver(observer, new CharacterVelocity() { VelocityY = - 10f}, new NotificationContext() { SubjectType = typeof(PlayerJumpController).ToString()}, cancellationToken));
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerStateBundle.StateBundle = data.StateBundle;
    }

    public async Task<ActionExecuted> PerformAction(bool value)
    {
        _isJumpPressed = value;

        await SetPlayerInitialPosition(PlayerStateBundle.StateBundle.PlayerMovementState);

        await HandleJumping();

        await HandleFalling();

        return new ActionExecuted() { Result = _isJumpPressed };
    }

    public async Task<ActionExecuted> CancelAction(bool value)
    {
        PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool> { CurrentState = LeapState.IS_JUMPING, CurrentValue = false, IsConcluded = true };

        await PlayerStateEvent.Invoke(PlayerStateBundle);

        return new ActionExecuted() { Result = false };
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
