using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiverEnhancedAsync<JumpingController, bool>, ISubject<IObserver<CharacterVelocity>>, IObserver<GenericStateBundle<PlayerStateBundle>>
{
    [SerializeField] LayerMask groundLayer;

    [SerializeField] LayerMask ledgeLayer;

    [SerializeField] float JumpSpeed;

    [SerializeField] float maxJumpHeight;

    private const float FALLING_SPEED = 0.8f;

    private const float JUMPING_SPEED = 0.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    
    private const float MAX_JUMP_TIME = 0.3f;

    private Rigidbody2D _rb;

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private Collider2D _col;

    private bool _isJumpPressed;

    private Vector3 _playerInitialPosition;

    public PlayerJumpTimeEvent onPlayerJumpTimeEvent;

    public CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity();

    public Vector3 PlayerInitialPosition { get => _playerInitialPosition; set=> _playerInitialPosition = value; }

    public float TimeEclipsed { get; set; }

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle> { };

    private void Awake()
    {
        //should give that one singe type of controller - and i think i we should move with this approach! Give me a moment that implements ActionExecuted!
        _animationReceiver = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>(_animationReceiver);

        _movementHelperClass = new MovementHelperClass();

        _col = GetComponent<CapsuleCollider2D>();

        _rb = GetComponent<Rigidbody2D>();

        PlayerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = Helper.GetCustomEvent<PlayerStateEvent>();

        PlayerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();
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

        PlayerVelocityDelegator.AddToSubjectsDict(typeof(JumpingController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());

        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[name].SetSubject(this);

    }

    //REMOVE THIS! -OR FIND A BETTER WAY TO REFACTOR THIS
    private async void Update()
    {
        await HandleJumpingMechanism();

        //no grabbing - since all of them are under a single state now
        if (PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState.Equals(PlayerMovementState.IS_JUMPING))
        {
            TimeEclipsed += Time.deltaTime;
        }

        onPlayerJumpTimeEvent.ShouldFall(TimeEclipsed);
    }
    public async Task HandleJumpingMechanism()
    {
        await HandleJumping();

        await HandleFalling();

        PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(JumpingController), CancellationToken.None);

        await Task.FromResult(true);
    }

    public async Task HandleFalling()
    {
        if (await CanPlayerFall(maxJumpHeight) || !_isJumpPressed || onPlayerJumpTimeEvent.Fall) //falling
        {
            CharacterVelocity.VelocityY = -JumpSpeed * FALLING_SPEED;
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && await IsYVelocityNegative(_rb))
        {
            await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>() { ExecutionState = PlayerAnimationExecutionState.PLAY_JUMPING_ANIMATION, Value = false });
        }

        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed) //on the ground
        {
            PlayerStateBundle.StateBundle.PlayerActionState = new State<PlayerActionState>() { CurrentState = PlayerActionState.IS_GRABBING, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            onPlayerJumpTimeEvent.Fall = false;

            TimeEclipsed = 0;
        }

        await Task.FromResult(true);
    }

    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) //jumping
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_JUMPING, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED;

            await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>() { ExecutionState = PlayerAnimationExecutionState.PLAY_JUMPING_ANIMATION, Value = true });
        }

    }

    private Task<bool> CanPlayerJump()
    {
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer));
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState != PlayerMovementState.IS_JUMPING, isOnLedgeOrGround, isJumpPressed));
    }

    private Task SetPlayerInitialPosition(State<PlayerMovementState> currentPlayerState)
    {
        if((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !currentPlayerState.CurrentState.Equals(PlayerMovementState.IS_JUMPING))
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
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    private bool IsOnTheLedge(LayerMask ledge)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
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
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[gameObject.name], observer);

        StartCoroutine(PlayerVelocityDelegator.NotifyObserver(observer, new CharacterVelocity() { VelocityY = - 10f}, new NotificationContext() { SubjectType = typeof(JumpingController).ToString()}, cancellationToken));
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerStateBundle.StateBundle = data.StateBundle;
    }

    public async Task<ActionExecuted<bool>> PerformAction(bool value)
    {
        _isJumpPressed = value;

        await SetPlayerInitialPosition(PlayerStateBundle.StateBundle.PlayerMovementState);

        return new ActionExecuted<bool>(_isJumpPressed);
    }

    public async Task<ActionExecuted<bool>> CancelAction()
    {
        PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState> { CurrentState = PlayerMovementState.IS_JUMPING, IsConcluded = true };

        await PlayerStateEvent.Invoke(PlayerStateBundle);

        return new ActionExecuted<bool>(false);
    }
}
