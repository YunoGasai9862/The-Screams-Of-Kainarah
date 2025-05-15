using CoreCode;
using NUnit.Framework.Internal;
using PlayerAnimationHandler;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[GameState(typeof(AttackingController))]
public class AttackingController : MonoBehaviour, IReceiver<bool>, IGameStateListener
{
    private const float TIME_DIFFERENCE_MAX = 1.5f;
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private Animator _anim;
    private Collider2D col;
    private MovementHelperClass _movementHelper;
    private bool _isPlayerEligibleForStartingAttack = false;
    private float timeDifferencebetweenStates;

    private GameState CurrentGameState { get; set; }

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

    private MouseClickEvent _onMouseClickEvent = new MouseClickEvent();

    private PlayerBoostAttackEvent _playerBoostAttackEvent = new PlayerBoostAttackEvent();

    public int PlayerAttackState { get; set; }
    public string PlayerAttackStateName { get; set; }
    public bool LeftMouseButtonPressed { get; set; }
    public bool BoostKeyPressed { get; set; } 
    public bool ShouldBoost { get; set; }
    public bool PowerUpBarFilled { get; set; } = false;
    public PlayerAttackStateMachine PlayerAttackStateMachine { get; set; }    

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        PlayerAttackStateMachine = new PlayerAttackStateMachine(_anim);

        col = GetComponent<CapsuleCollider2D>();

        _movementHelper = new MovementHelperClass();

        PlayerAttackState = 0;
    }

    private void Start()
    {
        //event subscription
        _onMouseClickEvent.AddListener(SetMouseClickBeginEndTime);
        _playerBoostAttackEvent.AddListener(SetAttackBoostMode);

        //Monobehavior event
        powerUpBarFillEvent.AddListener(PowerUpFillMode);
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerVariables.Instance.IS_SLIDING || PlayerAttackStateMachine.IstheAttackCancelConditionTrue(PlayerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
        {
            ResetAttackingState();
        }
    }
    private void InitiatePlayerAttack()
    {
        if (CanPlayerAttack()) //ground attack
        {
            PlayerVariables.Instance.attackVariableEvent.Invoke(true);
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
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnTheGround = _movementHelper.OverlapAgainstLayerMaskChecker(ref col, Ground, COLLIDER_DISTANCE_FROM_THE_LAYER);

        return isJumping && !isOnTheGround;
    }

    private void EndPlayerAttack()
    {
        _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

        PlayerVariables.Instance.attackVariableEvent.Invoke(false);

        PlayerAttackStateMachine.SetAttackState(jumpAttackStateName, PlayerVariables.Instance.IS_ATTACKING); //no jump attack

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
        PlayerVariables.Instance.attackVariableEvent.Invoke(false);
        PlayerAttackStateMachine.CanAttack(canAttackStateName, PlayerVariables.Instance.IS_ATTACKING);
        PlayerAttackStateMachine.CanAttack(jumpAttackStateName, PlayerVariables.Instance.IS_ATTACKING);
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
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = SceneSingleton.GetInventoryManager().IsPouchOpen;

        return !CurrentGameState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !isBuying && !isInventoryOpen && !isJumping;
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

    public Task Ping(GameState gameState)
    {
        CurrentGameState = gameState;

        return Task.CompletedTask;
    }
}
