using CoreCode;
using System;
using System.Threading;
using UnityEngine;
public class AttackingController : MonoBehaviour, IReceiver<bool>, IObserver<GenericState<GameState>>, IObserver<GenericState<PlayerState>>
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

    private GenericState<GameState> CurrentGameState { get; set; } = new GenericState<GameState>();
    private int PlayerAttackState { get; set; }
    private string PlayerAttackStateName { get; set; }
    private bool LeftMouseButtonPressed { get; set; }
    private bool BoostKeyPressed { get; set; }
    private bool ShouldBoost { get; set; }
    private bool PowerUpBarFilled { get; set; } = false;
    private PlayerAttackStateMachine PlayerAttackStateMachine { get; set; }

    private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; }

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

        PlayerAttackState = 0;

        GlobalGameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();
    }

    private void Start()
    {
        StartCoroutine(GlobalGameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(GameStateConsumer).ToString()
        }, CancellationToken.None));


        StartCoroutine(PlayerSystemDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerSystem).ToString()
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
        if (PlayerSystem == null)
        {
            return;
        }

        if (PlayerSystem.IS_SLIDING || PlayerAttackStateMachine.IstheAttackCancelConditionTrue(PlayerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
        {
            ResetAttackingState();
        }
    }
    private void InitiatePlayerAttack()
    {
        if (CanPlayerAttack()) //ground attack
        {
            PlayerSystem.attackVariableEvent.Invoke(true);
            //keeps track of attacking state
            (PlayerAttackState, PlayerAttackStateName, _isPlayerEligibleForStartingAttack) = GetEnumStateAndName<PlayerAttackEnum.PlayerAttackSlash>( PlayerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

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
        bool isJumping = PlayerSystem.IS_JUMPING;
        bool isOnTheGround = _movementHelper.OverlapAgainstLayerMaskChecker(ref col, Ground, COLLIDER_DISTANCE_FROM_THE_LAYER);

        return isJumping && !isOnTheGround;
    }

    private void EndPlayerAttack()
    {
        _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

        PlayerSystem.attackVariableEvent.Invoke(false);

        PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, PlayerSystem.IS_ATTACKING); //no jump attack

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
            PlayerAttackStateMachine.SetAttackState(attackStateName, PlayerAttackState); //toggles state

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
        PlayerAttackState = 0; //resets the attackingstate
        PlayerSystem.attackVariableEvent.Invoke(false);
        PlayerAttackStateMachine.CanAttack(canAttackStateName, PlayerSystem.IS_ATTACKING);
        PlayerAttackStateMachine.CanAttack(jumpAttackStateName, PlayerSystem.IS_ATTACKING);
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
        bool isJumping = PlayerSystem.IS_JUMPING;
        bool isInventoryOpen = SceneSingleton.GetInventoryManager().IsPouchOpen;

        return !CurrentGameState.State.Equals(GameState.DIALOGUE_TAKING_PLACE) &&
               !CurrentGameState.State.Equals(GameState.SHOPPING) && !isInventoryOpen && !isJumping;
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
    public bool PerformAction(bool value)
    {
        LeftMouseButtonPressed = value;
        InitiatePlayerAttack();
        return true;
    }

    public bool CancelAction()
    {
        EndPlayerAttack();
        return true;
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
        if (BoostKeyPressed && PowerUpBarFilled && PlayerAttackState >= 0)
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

    public void OnNotify(GenericState<GameState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public void OnNotify(GenericState<PlayerState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new NotImplementedException();
    }
}
