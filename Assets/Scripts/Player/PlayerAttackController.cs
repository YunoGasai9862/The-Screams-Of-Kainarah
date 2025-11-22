using CoreCode;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour, IReceiverEnhancedAsync<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>>, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<GenericStateBundle<GameStateBundle>>, IObserver<Player>
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

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }
    
    private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; } 

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private PlayerBoostAttackEvent PlayerBoostAttackEvent { get; set; }

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

        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        PlayerBoostAttackEvent = await Helper.GetCustomEvent<PlayerBoostAttackEvent>();    

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        GlobalGameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();
    }

    private void Start()
    {
        StartCoroutine(GlobalGameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GameStateConsumer).ToString()

        }, CancellationToken.None));

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
        return (CurrentPlayerState.StateBundle.PlayerMovementState.CurrentState == MovementState.IS_JUMPING && !CurrentPlayerState.StateBundle.PlayerMovementState.IsConcluded) && 
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


        bool isInventoryOpen = SceneSingleton.GetInventoryManager().IsPouchOpen;

        return CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.FREE_MOVEMENT) &&
               !CurrentGameState.StateBundle.GameState.IsConcluded &&
               !isInventoryOpen &&
               !CurrentPlayerState.StateBundle.PlayerMovementState.CurrentState.Equals(MovementState.IS_JUMPING);
    }

    public Task<ActionExecuted<ControllerPackage<AttackingExecutionState, AttackingDetails>>> PerformAction(ControllerPackage<AttackingExecutionState, AttackingDetails> value)
    {
        if (CurrentGameState.StateBundle == null)
        {
            Debug.Log("Bundle is null - skipping CanPlayerAttack!");

            return null;
        }

        DelegateExecutionState(value);
        
        return Task.FromResult(new ActionExecuted<ControllerPackage<AttackingExecutionState, AttackingDetails>>(value));
    }

    public Task<ActionExecuted<ControllerPackage<AttackingExecutionState, AttackingDetails>>> CancelAction(ControllerPackage<AttackingExecutionState, AttackingDetails> value)
    {
        EndPlayerAttack();

        return Task.FromResult(new ActionExecuted<ControllerPackage<AttackingExecutionState, AttackingDetails>>(
              new ControllerPackage<AttackingExecutionState, AttackingDetails>()
              {
                  ExecutionState = AttackingExecutionState.CANCELLED,
                  
              }
            )
         );
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

    public void OnNotify(GenericStateBundle<GameStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState.StateBundle = data.StateBundle;
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentPlayerState.StateBundle = data.StateBundle;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;

        PlayerAttackStateMachine = new PlayerAttackStateMachine(Player.Animator);
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
}
