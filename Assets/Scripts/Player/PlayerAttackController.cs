using Assets.Annotations;
using CoreCode;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Threading.Tasks;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerStateConsumer), ObserverType = typeof(PlayerAttackController), ContextType = typeof(GenericStateBundle<PlayerStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(GameStateConsumer), ObserverType = typeof(PlayerAttackController), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(PlayerAttackController), ContextType = typeof(Player))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(InventoryManager), ContextType = typeof(InventoryManager))]
public class PlayerAttackController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>>, INotify<GenericStateBundle<PlayerStateBundle>>, INotify<GenericStateBundle<GameStateBundle>>, INotify<InventoryManager>, INotify<Player>
{
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private MovementHelperClass _movementHelper;
    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>();
    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();
    private int PlayerAttackStateInt { get; set; }    private bool LeftMouseButtonPressed { get; set; }
    private bool BoostKeyPressed { get; set; }
    private bool ShouldBoost { get; set; }
    private bool PowerUpBarFilled { get; set; } = false;
    private PlayerAttackStateMachine PlayerAttackStateMachine { get; set; }

    private Delegator Delegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private PlayerBoostAttackEvent PlayerBoostAttackEvent { get; set; }

    private InventoryManager InventoryManagerInstance { get; set; }

    private MouseClickDto MouseClickDto { get; set; }

    private Player Player { get; set; }
        
    [SerializeField] LayerMask Ground;

    [SerializeField] LayerMask ledge;

    [SerializeField] string canAttackStateName;

    [SerializeField] string attackStateName;

    [SerializeField] string timeDifferenceStateName;

    [SerializeField] string jumpAttackStateName;

    [SerializeField] string booksAttackStateName;

    [SerializeField] PowerUpBarFillEvent powerUpBarFillEvent;


    private async void Awake()
    {
        _movementHelper = new MovementHelperClass();

        PlayerAttackStateInt = 0;

        Delegator = await Helper.GetDelegator<Delegator>();

        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        PlayerBoostAttackEvent = await Helper.GetCustomEvent<PlayerBoostAttackEvent>();
    }

    private void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<GameStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerAttackController),
            SubjectType = typeof(GameStateConsumer)

        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<PlayerStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerAttackController),
            SubjectType = typeof(PlayerStateConsumer)
        }, this));


        StartCoroutine(Delegator.NotifySubject(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerAttackController),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));

        PlayerBoostAttackEvent.AddListener(SetAttackBoostMode);

        powerUpBarFillEvent.AddListener(PowerUpFillMode);
    }

    private void InitiatePlayerAttack(bool leftMouseButtonPressed)
    {
        LeftMouseButtonPressed = leftMouseButtonPressed;

        if (CanPlayerAttack())
        {
            CurrentPlayerState.StateBundle.PlayerAttackState = new State<AttackState, bool>() { CurrentState = AttackState.IS_ATTACKING,  CurrentValue = true, IsConcluded = false };

            PlayerStateEvent.Invoke(CurrentPlayerState);

            PlayerAttackStateMachine.CanAttack(canAttackStateName, LeftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>(LeftMouseButtonPressed);

        }

        if (CanPlayerAttackWhileJumping())
        {
            PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, LeftMouseButtonPressed);
        }
    }
    private bool CanPlayerAttackWhileJumping()
    {
        return (CurrentPlayerState.StateBundle.PlayerLeapState.CurrentState == LeapState.IS_JUMPING && !CurrentPlayerState.StateBundle.PlayerMovementState.IsConcluded) && 
            !_movementHelper.OverlapAgainstLayerMaskChecker(Player.Collider, Ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }

    private void EndPlayerAttack()
    {
        if (CurrentPlayerState.StateBundle == null)
        {
            Debug.Log("CurrentPlayerState.StateBundle is null - skipping EndPlayerAttack!");

            return;
        }

        CurrentPlayerState.StateBundle.PlayerAttackState = new State<AttackState, bool>() { CurrentState = AttackState.IS_ATTACKING, CurrentValue = true, IsConcluded = false };

        PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, (int)CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState); //no jump attack
    }
   
    private void PlayerAttackMechanism<T>(bool isPlayerEligibleForStartingAttack)
    {
        if (isPlayerEligibleForStartingAttack) //cast Type <T>
        {
            PlayerAttackStateMachine.SetAttackState(attackStateName, PlayerAttackStateInt); //toggles state

            PlayerAttackStateMachine.TimeDifferenceRequiredBetweenTwoStates(timeDifferenceStateName, MouseClickDto.TimeDifference); //keeps track of time elapsed

        }
    }

    public bool CanPlayerAttack()
    {
        if (CurrentGameState.StateBundle == null)
        {
            Debug.Log("Bundle is null - skipping CanPlayerAttack!");

            return false;
        }

        if (CurrentGameState.StateBundle.GameState == null)
        {
            Debug.Log("StateBundle.GameState is null - skipping CanPlayerAttack!");

            return false;
        }


        bool isInventoryOpen = InventoryManagerInstance == null ? false : InventoryManagerInstance.IsPouchOpen;

        return CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.FREE_MOVEMENT) &&
               !CurrentGameState.StateBundle.GameState.IsConcluded &&
               !isInventoryOpen &&
               !CurrentPlayerState.StateBundle.PlayerLeapState.CurrentState.Equals(LeapState.IS_JUMPING);
    }

    public Task<ActionExecuted> PerformAction(ControllerPackage<AttackingExecutionState, AttackingDetails> value)
    {
        if (CurrentGameState.StateBundle == null)
        {
            Debug.Log("Bundle is null - skipping CanPlayerAttack!");

            return Task.FromResult(new ActionExecuted() { Result = false });
        }

        DelegateExecutionState(value);

        return Task.FromResult(new ActionExecuted() { Result = true });
    }

    public Task<ActionExecuted> CancelAction(ControllerPackage<AttackingExecutionState, AttackingDetails> value)
    {
        EndPlayerAttack();

        return Task.FromResult(new ActionExecuted() { Result = false });

    }
    private void SetAttackBoostMode(bool shouldBoost)
    {
        PlayerAttackStateMachine.SetAttackState(booksAttackStateName, shouldBoost);
    }

    public void AlertBoostEventForKeyPressed(bool keyPressed)
    {
        BoostKeyPressed = keyPressed;
        if (BoostKeyPressed && PowerUpBarFilled && PlayerAttackStateInt >= 0)
        {
            ShouldBoost = true;
        }
        else
            ShouldBoost = false;

        PlayerBoostAttackEvent.Invoke(ShouldBoost);
    }
    public void PowerUpFillMode(bool filledUp)
    {
        PowerUpBarFilled = filledUp;
    }

    private void DelegateExecutionState(ControllerPackage<AttackingExecutionState, AttackingDetails> controllerPackage)
    {
        switch(controllerPackage.ExecutionState)
        {
            case AttackingExecutionState.ON_CLICK_EVENT:
                MouseClickDto = controllerPackage.Value.MouseClickDto;
                break;

            case AttackingExecutionState.ATTACKING_ACTION:
                InitiatePlayerAttack(controllerPackage.Value.AttackingValue);
                break;

            case AttackingExecutionState.BOOST_ATTACK:
                AlertBoostEventForKeyPressed(controllerPackage.Value.AttackingValue);
                break;

            case AttackingExecutionState.CANCELLED:
                break;

            default:
                break;
        }
    }

    public IEnumerator Notify(GenericStateBundle<PlayerStateBundle> value)
    {
        CurrentPlayerState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        CurrentGameState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        PlayerAttackStateMachine = new PlayerAttackStateMachine(Player.Animator);

        yield return null;
    }

    public IEnumerator Notify(InventoryManager value)
    {
        InventoryManagerInstance = value;

        yield return null;
    }
}
