using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour, IReceiverEnhancedAsync<PlayerJumpController, bool>, ISubject<CharacterVelocity>, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<Player>
{
    [SerializeField] LayerMask groundLayer;

    [SerializeField] LayerMask ledgeLayer;

    [SerializeField] float JumpSpeed;

    [SerializeField] float maxJumpHeight;

    private const float FALLING_SPPED_RATIO = 0.8f;

    private const float JUMPING_SPEED_RATIO = 0.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    
    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationCommand;

    private IOverlapChecker _movementHelperClass;

    private CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity();

    private Player Player { get; set; }

    public Vector3 PlayerInitialPosition { get; set; }

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
        StartCoroutine(PlayerStateDelegator.NotifySubject(this, new ObserverContext()
        {
            Name = gameObject.name,
            Tag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer)
        }, CancellationToken.None));

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new ObserverContext()
        {
            Name = gameObject.name,
            Tag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier)
        }, CancellationToken.None));

        PlayerVelocityDelegator.AddToSubjectsDict(typeof(PlayerJumpController).ToString(), gameObject.name, new Subject<CharacterVelocity>(this, typeof(PlayerJumpController)));
    }

    public async Task HandleJumping(bool canJump)
    {
        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && canJump) 
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_JUMPING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED_RATIO;

            PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, CancellationToken.None);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.LEAP, Value = PlayerStateBundle.StateBundle});
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && IsAtMaxJumpHeight(maxJumpHeight))
        {
            Debug.Log($"HandleFalling - CanPlayerFall");

            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_FALLING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = (-1) * JumpSpeed * FALLING_SPPED_RATIO;

            PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, CancellationToken.None);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.LEAP, Value = PlayerStateBundle.StateBundle });
        }
    }

    private bool IsOnTheGround(LayerMask ground)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    private bool IsOnTheLedge(LayerMask ledge)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    public bool IsAtMaxJumpHeight(float maxJumpHeight)
    {
        Debug.Log("IsAtMaxJumpHeight: " + (Player.Transform.position.y) + " " + (PlayerInitialPosition.y + maxJumpHeight));
        return Player.Transform.position.y >= PlayerInitialPosition.y + maxJumpHeight;
    }

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.CreateAssociation(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(PlayerJumpController).ToString())[gameObject.name], observer);

        StartCoroutine(PlayerVelocityDelegator.NotifyObserver(observer, new CharacterVelocity() { VelocityY = - 10f}, new ObserverContext() { SubjectType = typeof(PlayerJumpController)}, cancellationToken));
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerStateBundle.StateBundle = data.StateBundle;
    }

    public async Task<ActionExecuted> PerformAction(bool value)
    {
        PlayerInitialPosition = Player.Transform.position;

        await HandleJumping(value);

        return new ActionExecuted() { Result = value };
    }

    public async Task<ActionExecuted> CancelAction(bool value)
    {
        CharacterVelocity.VelocityY = (-1) * JumpSpeed * FALLING_SPPED_RATIO;

        PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, CancellationToken.None);

        return new ActionExecuted() { Result = false };
    }

    public void OnNotify(Player data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
