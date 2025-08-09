using CoreCode;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AttackingController : MonoBehaviour, IReceiverEnhancedAsync<AttackingController, ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<GenericStateBundle<GameStateBundle>>
{
    private const float TIME_DIFFERENCE_MAX = 1.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private MouseClickEvent _onMouseClickEvent = new MouseClickEvent();

    private PlayerBoostAttackEvent _playerBoostAttackEvent = new PlayerBoostAttackEvent();

    private Animator _anim;

    private Collider2D col;

    private MovementHelperClass _movementHelper;

    private bool _isPlayerEligibleForStartingAttack = false;

    private float timeDifferencebetweenStates;
    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>();
    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();
    private int PlayerAttackStateInt { get; set; }
    private string PlayerAttackStateName { get; set; }
    private bool LeftMouseButtonPressed { get; set; }
    private bool BoostKeyPressed { get; set; }
    private bool ShouldBoost { get; set; }
    private bool PowerUpBarFilled { get; set; } = false;
    private PlayerAttackStateMachine PlayerAttackStateMachine { get; set; }

    private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; }

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }
        
    [SerializeField] LayerMask Ground;

    [SerializeField] LayerMask ledge;

    [SerializeField] GameObject IceTrail;

    [SerializeField] GameObject IceTrail2;

    [SerializeField] string canAttackStateName;

    [SerializeField] string attackStateName;

    [SerializeField] string timeDifferenceStateName;

    [SerializeField] string jumpAttackStateName;

    [SerializeField] string booksAttackStateName;

    [SerializeField] PowerUpBarFillEvent powerUpBarFillEvent;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        PlayerAttackStateMachine = new PlayerAttackStateMachine(_anim);

        col = GetComponent<CapsuleCollider2D>();

        _movementHelper = new MovementHelperClass();

        PlayerAttackStateInt = 0;

        GlobalGameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        PlayerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = Helper.GetCustomEvent<PlayerStateEvent>();
    }

    private void Start()
    {
        StartCoroutine(GlobalGameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(GameStateConsumer).ToString()
        }, CancellationToken.None));


        StartCoroutine(PlayerStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None));


        //event subscription
        _onMouseClickEvent.AddListener(SetMouseClickBeginEndTime);
        _playerBoostAttackEvent.AddListener(SetAttackBoostMode);

        //Monobehavior event
        powerUpBarFillEvent.AddListener(PowerUpFillMode);
    }
    // Update is called once per frame
    void Update()
    {
        if (CurrentPlayerState.StateBundle == null)
        {
            Debug.Log("CurrentPlayerState.StateBundle is null - skipping update!");
        }

        if (CurrentPlayerState.StateBundle.PlayerMovementState.CurrentState.Equals(PlayerMovementState.IS_SLIDING) || 
            PlayerAttackStateMachine.IstheAttackCancelConditionTrue(PlayerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
        {
            ResetAttackingState();
        }
    }
    private void InitiatePlayerAttack(bool leftMouseButtonPressed)
    {
        LeftMouseButtonPressed = leftMouseButtonPressed;

        if (CanPlayerAttack()) //ground attack
        {
            CurrentPlayerState.StateBundle.PlayerAttackState = new State<PlayerAttackState>() { CurrentState = PlayerAttackState.IS_ATTACKING, IsConcluded = false };

            PlayerStateEvent.Invoke(CurrentPlayerState);

            //keeps track of attacking state
            (PlayerAttackStateInt, PlayerAttackStateName, _isPlayerEligibleForStartingAttack) = GetEnumStateAndName<PlayerAttackEnum.PlayerAttackSlash>( PlayerAttackStateInt, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

            //sets the initial configuration for the attacking system
            PlayerAttackStateMachine.CanAttack(canAttackStateName, LeftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>(_isPlayerEligibleForStartingAttack);

        }

        if (CanPlayerAttackWhileJumping())
        {
            PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, LeftMouseButtonPressed);
        }
    }
    private bool CanPlayerAttackWhileJumping()
    {
        return (CurrentPlayerState.StateBundle.PlayerMovementState.CurrentState == PlayerMovementState.IS_JUMPING && CurrentPlayerState.StateBundle.PlayerMovementState.IsConcluded) && 
            !_movementHelper.OverlapAgainstLayerMaskChecker(ref col, Ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }

    private void EndPlayerAttack()
    {
        _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

        CurrentPlayerState.StateBundle.PlayerAttackState = new State<PlayerAttackState>() { CurrentState = PlayerAttackState.IS_ATTACKING, IsConcluded = false };

        PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, (int)CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState); //no jump attack

    }
    private (int, string, bool) GetEnumStateAndName<T>(int playerAttackState, int initialStateOfTheEnum)
    {
        int enumSize = GetLenthofEnum<T>(); //returns the length of the Enum
        string playerAttackStateName = String.Empty;

        foreach (T PAS in Enum.GetValues(typeof(T)))
        {
            int dummy = Convert.ToInt32(PAS) - 1; //converts to INTEnumStateManipulator

            if (playerAttackState == dummy)
            {
                dummy++;

                playerAttackState = dummy <= enumSize ? dummy : initialStateOfTheEnum; //sets to the initial State of the Enum

                playerAttackStateName = Enum.GetName(typeof(T), playerAttackState);

                return (playerAttackState, playerAttackStateName, true);
            }
        }

        return (playerAttackState, playerAttackStateName, false);
    }

    private void PlayerAttackMechanism<T>(bool isPlayerEligibleForStartingAttack)
    {
        if (isPlayerEligibleForStartingAttack) //cast Type <T>
        {
            PlayerAttackStateMachine.SetAttackState(attackStateName, PlayerAttackStateInt); //toggles state

            timeDifferencebetweenStates = _onMouseClickEvent.TimeDifferenceBetweenPressAndRelease();

            PlayerAttackStateMachine.TimeDifferenceRequiredBetweenTwoStates(timeDifferenceStateName, timeDifferencebetweenStates);     //keeps track of time elapsed

        }

        if (IsEnumValueEqualToLengthOfEnum<T>(PlayerAttackStateName) || (IsTimeDifferenceWithinRange(timeDifferencebetweenStates, TIME_DIFFERENCE_MAX) &&
            PlayerAttackStateName != PlayerAttackStateMachine.GetStateNameThroughEnum(1))) //dont do it for the first attackState
        {
            ResetAttackingState();
            return;
        }
    }
    private bool IsTimeDifferenceWithinRange(float value, float upperBoundary)
    {
        return value > upperBoundary;
    }
    private void ResetAttackingState()
    {
        PlayerAttackStateInt = 0; //resets the attackingstate

        CurrentPlayerState.StateBundle.PlayerAttackState = new State<PlayerAttackState>() { CurrentState = PlayerAttackState.IS_ATTACKING, IsConcluded = true };

        PlayerStateEvent.Invoke(CurrentPlayerState);

        //use this now - is concluded to track if the player can resume attacking again :))
        PlayerAttackStateMachine.CanAttack(canAttackStateName, CurrentPlayerState.StateBundle.PlayerAttackState.IsConcluded);

        PlayerAttackStateMachine.CanAttack(jumpAttackStateName, CurrentPlayerState.StateBundle.PlayerAttackState.IsConcluded);
    }

    private bool IsEnumValueEqualToLengthOfEnum<T>(string _playerAttackStateName)
    {
        return (int)Enum.Parse(typeof(T), _playerAttackStateName) == GetLenthofEnum<T>();
    }

    private int GetLenthofEnum<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }
    public bool CanPlayerAttack()
    {
        bool isInventoryOpen = SceneSingleton.GetInventoryManager().IsPouchOpen;

        return !CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE) &&
               !CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.SHOPPING) && !isInventoryOpen &&
               !CurrentPlayerState.StateBundle.PlayerMovementState.CurrentState.Equals(PlayerMovementState.IS_JUMPING);
    }

    #region AnimationEventOnTheAnimationItself
    public void Icetail()
    {
        InstantiatorController iceTrail = new(IceTrail);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.SetGameObjectParent(transform);
    }

    public void Icetail2()
    {
        InstantiatorController iceTrail = new(IceTrail2);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.SetGameObjectParent(transform);
    }

    public void BoostAttackStateManagement()
    {
        ShouldBoost = false;
        _playerBoostAttackEvent.Invoke(ShouldBoost);
    }

    #endregion
    public Task<ActionExecuted<ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>> PerformAction(ControllerPackage<PlayerAttackingExecutionState, AttackingDetails> value)
    {
        DelegateExecutionState(value);
        
        return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>(value));
    }

    public Task<ActionExecuted<ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>> CancelAction(ControllerPackage<PlayerAttackingExecutionState, AttackingDetails> value)
    {
        EndPlayerAttack();

        return Task.FromResult(new ActionExecuted<ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>(
              new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
              {
                  ExecutionState = PlayerAttackingExecutionState.CANCELLED,
                  
              }
            )
         );
    }
    private void SetAttackBoostMode(bool shouldBoost)
    {
        PlayerAttackStateMachine.SetAttackState(booksAttackStateName, shouldBoost);
    }

    public void SetMouseClickBeginEndTime(float startTime, float endTime)
    {
        _onMouseClickEvent.ClickStartTime = startTime;
        _onMouseClickEvent.ClickEndTime = endTime;
    }
    public void InvokeOnMouseClickEvent(float startTime, float endTime)
    {
        _onMouseClickEvent.Invoke(startTime, endTime);
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

        _playerBoostAttackEvent.Invoke(ShouldBoost);
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
        Debug.Log($"PlayerStateBundle in Attacking Controller - {data.StateBundle}");

        CurrentPlayerState.StateBundle = data.StateBundle;
    }

    private void DelegateExecutionState(ControllerPackage<PlayerAttackingExecutionState, AttackingDetails> controllerPackage)
    {
        switch(controllerPackage.ExecutionState)
        {
            case PlayerAttackingExecutionState.ON_CLICK_EVENT:
                InvokeOnMouseClickEvent(controllerPackage.Value.AttackingStartTime, controllerPackage.Value.AttackingEndTime);
                break;

            case PlayerAttackingExecutionState.ATTACKING_ACTION:
                InitiatePlayerAttack(controllerPackage.Value.AttackingValue);
                break;

            case PlayerAttackingExecutionState.BOOST_ATTACK:
                AlertBoostEventForKeyPressed(controllerPackage.Value.AttackingValue);
                break;

            case PlayerAttackingExecutionState.CANCELLED:
                break;

            default:
                break;
        }
    }
}
