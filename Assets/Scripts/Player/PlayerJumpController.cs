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

    private const float FALLING_SPEED = 0.8f;

    private const float JUMPING_SPEED = 0.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    
    private const float MAX_JUMP_TIME = 0.3f;

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private bool _isJumpPressed;

    private Vector3 _playerInitialPosition;

    public PlayerJumpTimeEvent onPlayerJumpTimeEvent;

    public CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity();

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
        onPlayerJumpTimeEvent = new PlayerJumpTimeEvent(MAX_JUMP_TIME);

        onPlayerJumpTimeEvent.AddListener(MaxTimePassed);

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

    private async void Update()
    {
        if (Player == null)
        {
            Debug.Log("Player is null - skipping update for Jumping Controller!");
            return;
        }

        await HandleJumpingMechanism();

        //no grabbing - since all of them are under a single state now
        if (PlayerStateBundle.StateBundle.PlayerLeapState.CurrentState.Equals(LeapState.IS_JUMPING))
        {
            TimeEclipsed += Time.deltaTime;
        }

        onPlayerJumpTimeEvent.ShouldFall(TimeEclipsed);
    }
    public async Task HandleJumpingMechanism()
    {
        //THE PROBLEM IS WE ARE TRYING TO NOTIFY BEFORE WE HAVE THE DICT - FIX IT!!
        await HandleJumping();

        await HandleFalling();

        PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(PlayerJumpController), CancellationToken.None);

        await Task.FromResult(true);
    }

    public async Task HandleFalling()
    {
        if (await CanPlayerFall(maxJumpHeight) || !_isJumpPressed || onPlayerJumpTimeEvent.Fall) //falling
        {
            CharacterVelocity.VelocityY = -JumpSpeed * FALLING_SPEED;
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && await IsYVelocityNegative(Player.Rigidbody))
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_FALLING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle });
        }

        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed) //on the ground
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IDLE, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle });

            onPlayerJumpTimeEvent.Fall = false;

            TimeEclipsed = 0;
        }

        await Task.FromResult(true);
    }

    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) 
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_JUMPING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED;

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.IN_AIR, Value = PlayerStateBundle.StateBundle});
        }

    }

    private Task<bool> CanPlayerJump()
    {
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer));
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(PlayerStateBundle.StateBundle.PlayerLeapState.CurrentState != LeapState.IS_JUMPING, isOnLedgeOrGround, isJumpPressed));
    }

    private Task SetPlayerInitialPosition(State<MovementState, bool> currentPlayerState)
    {
        if((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !currentPlayerState.CurrentState.Equals(LeapState.IS_JUMPING))
        {
            PlayerInitialPosition = transform.position;
        }
        return Task.CompletedTask;
    }
    private async Task<bool> CanPlayerFall(float maxJumpHeight)
    {
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) && IsOnTheLedge(ledgeLayer));
        return MovementHelperFunctions.boolConditionAndTester(!isOnLedgeOrGround, await MaxJumpHeightChecker(maxJumpHeight));
    }
    private bool IsOnTheGround(LayerMask ground)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    private bool IsOnTheLedge(LayerMask ledge)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    public async Task<bool> MaxJumpHeightChecker(float maxJumpHeight)
    {
        if(transform.position.y >= PlayerInitialPosition.y + maxJumpHeight )
        {
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public void MaxTimePassed(bool value)
    {
        onPlayerJumpTimeEvent.Fall = value;
    }

    private Task<bool> IsYVelocityNegative(Rigidbody2D rb)
    {
        return rb.linearVelocity.y < 0 ? Task.FromResult(true) : Task.FromResult(false);
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
