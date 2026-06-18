using Assets.Annotations;
using CoreCode;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.Scene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerSlideController), SubjectType = typeof(PlayerAnimationController), ContextType = typeof(AnimationDetails))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerSlideController), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerSlideController), ContextType = typeof(CharacterVelocity))]
public class PlayerSlideController : MonoBehaviorScene, IReceiverEnhancedAsync<PlayerSlideController, PlayerStateBundle>, INotify<AnimationDetails>, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<CharacterVelocity>, INotify<Player>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float slidingSpeed;

    private Delegator Delegator { get; set; }

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

    private CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity() {VelocityX = 0f, VelocityY = 0f, VelocityZ = 0f };

    private bool IS_SLIDING { get; set; } = false;

    private async void Awake()
    {
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        PlayerStateEvent = await SceneUtils.GetCustomEvent<PlayerStateEvent>(); 
    }

    void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<AnimationDetails>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerSlideController),
            SubjectType = typeof(PlayerAnimationController),
        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerSlideController),
            SubjectType = typeof(PlayerAttributesNotifier),
        }, this));

        _animationHandler = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationHandler);

        _movementHelperClass = new MovementHelperClass();
    }

    private async Task Slide()
    {
        if (IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(Player.Collider, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = new MovementDto() { SlidingSpeed = new Vector2(slidingSpeed, 0) }, IsConcluded = false };

            CharacterVelocity.VelocityX = slidingSpeed;

            //we need to also notify here intentionally so we can pass the latest updated speed
            //TODO allow the delegator to batch notify
            Delegator.NotifyObserversWrapper(new SubjectContext<CharacterVelocity>() { EntityType = typeof(PlayerSlideController), Data = CharacterVelocity }, this);

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.INTERACTION, Value = PlayerStateBundle.StateBundle });
        }

        if (AnimationDetails.CurrentAnimationTime > MAX_ANIMATION_TIME && AnimationDetails.CurrentAnimationStateInfo.IsName(PlayerAnimationField.Sliding.ToString()))
        {
            PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = new MovementDto() { SlidingSpeed = new Vector2(slidingSpeed, 0) }, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerStateBundle);

            await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>() { ExecutionState = AnimationExecutionState.INTERACTION, Value = PlayerStateBundle.StateBundle });
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
        PlayerStateBundle.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = new MovementDto() { SlidingSpeed = Vector2.zero }, IsConcluded = true };

        CharacterVelocity.VelocityX = 0f;

        Delegator.NotifyObserversWrapper(new SubjectContext<CharacterVelocity>() { EntityType = typeof(PlayerSlideController), Data = CharacterVelocity }, this);

        await PlayerStateEvent.Invoke(PlayerStateBundle);

        return await Task.FromResult(new ActionExecuted() { Result = false });
    }

    private Task<bool> IsVelocityXGreaterThanZero(Rigidbody2D rb)
    {
        return Task.FromResult(Mathf.Abs(rb.linearVelocity.x) > 0);
    }

    public IEnumerator Notify(AnimationDetails value)
    {
        AnimationDetails = value;

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        _playerAttackStateMachine = new PlayerAttackStateMachine(value.Animator);

        yield return null;
    }

    public IEnumerator<CharacterVelocity> Request()
    {
       StartCoroutine(Delegator.NotifyObservers(new SubjectContext<CharacterVelocity>() { EntityType = typeof(PlayerSlideController), Data = CharacterVelocity }, this));

        yield return null;
    }
}
