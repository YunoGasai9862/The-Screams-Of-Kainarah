using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.Scene;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerJumpController), ContextType = typeof(CharacterVelocity))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerAttributesNotifier), EntityType = typeof(PlayerJumpController), ContextType = typeof(Player))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerStateConsumer), EntityType = typeof(PlayerJumpController), ContextType = typeof(GenericStateBundle<PlayerStateBundle>))]
public class PlayerJumpController : Scene, IReceiverEnhancedAsync<PlayerJumpController, bool>, IRequest<CharacterVelocity>, INotify<GenericStateBundle<PlayerStateBundle>>, INotify<Player>
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

    private Delegator Delegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle> { StateBundle = new PlayerStateBundle() };

    private SceneUtils SceneUtils { get; set; }

    private async void Awake()
    {
        SceneUtils = await BaseScene.GetSceneUtilsAsync();

        _movementHelperClass = new MovementHelperClass();

       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        PlayerStateEvent = await SceneUtils.GetCustomEvent<PlayerStateEvent>();

        _animationReceiver = await SceneUtils.FindReceiver<PlayerAnimationController, IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationReceiver);
    }
    private void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<PlayerStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerJumpController),
            SubjectType = typeof(PlayerStateConsumer)
        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerJumpController),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));
    }

    public async Task HandleJumping(bool canJump)
    {
        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && canJump) 
        {
            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_JUMPING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED_RATIO;

            Delegator.NotifyObserversWrapper(new SubjectContext<CharacterVelocity> { Data = CharacterVelocity, EntityType = typeof(PlayerJumpController) }, this);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.LEAP, Value = PlayerStateBundle.StateBundle});
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && IsAtMaxJumpHeight(maxJumpHeight))
        {
            Debug.Log($"HandleFalling - CanPlayerFall");

            PlayerStateBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_FALLING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            CharacterVelocity.VelocityY = (-1) * JumpSpeed * FALLING_SPPED_RATIO;

            Delegator.NotifyObserversWrapper(new SubjectContext<CharacterVelocity> { Data = CharacterVelocity, EntityType = typeof(PlayerJumpController) }, this);

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

    public async Task<ActionExecuted> PerformAction(bool value)
    {
        PlayerInitialPosition = Player.Transform.position;

        await HandleJumping(value);

        return new ActionExecuted() { Result = value };
    }

    //need to add a utility for NotifyObservers (in collection :)))
    public async Task<ActionExecuted> CancelAction(bool value)
    {
        CharacterVelocity.VelocityY = (-1) * JumpSpeed * FALLING_SPPED_RATIO;

        Delegator.NotifyObserversWrapper(new SubjectContext<CharacterVelocity> { Data = CharacterVelocity, EntityType = typeof(PlayerJumpController) }, this);

        return new ActionExecuted() { Result = false };
    }

    public IEnumerator Notify(GenericStateBundle<PlayerStateBundle> value)
    {
        PlayerStateBundle.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }

    public IEnumerator Request()
    {
       StartCoroutine(Delegator.NotifyObservers(new SubjectContext<CharacterVelocity> { Data = CharacterVelocity, EntityType = typeof(PlayerJumpController) }, this));

        yield return null;
    }
}
