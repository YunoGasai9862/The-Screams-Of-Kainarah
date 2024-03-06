using CoreCode;
using NUnit.Framework.Internal;
using PlayerAnimationHandler;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackingController : MonoBehaviour, IReceiver<bool>
{
    private const float TIME_DIFFERENCE_MAX = 1.5f;
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private Animator _anim;
    private Collider2D col;
    private MovementHelperClass _movementHelper;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private bool _isPlayerEligibleForStartingAttack = false;
    private float timeDifferencebetweenStates;

    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject IceTrail;
    [SerializeField] GameObject IceTrail2;
    [SerializeField] string canAttackStateName;
    [SerializeField] string attackStateName;
    [SerializeField] string timeDifferenceStateName;
    [SerializeField] string jumpAttackStateName;

    private MouseClickEvent onMouseClickEvent = new MouseClickEvent();
    private int PlayerAttackState { get; set; }
    private string PlayerAttackStateName { get; set; }
    private bool LeftMouseButtonPressed { get; set; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);

        col = GetComponent<CapsuleCollider2D>();

        _movementHelper = new MovementHelperClass();

        PlayerAttackState = 0;
    }

    private void Start()
    {
        //event subscription
        onMouseClickEvent.AddListener(SetMouseClickBeginEndTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerVariables.Instance.IS_SLIDING || _playerAttackStateMachine.IstheAttackCancelConditionTrue(PlayerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
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
            _playerAttackStateMachine.CanAttack(canAttackStateName, LeftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>(_isPlayerEligibleForStartingAttack);

        }

        if (CanPlayerAttackWhileJumping())
        {
            _playerAttackStateMachine.SetAttackState(jumpAttackStateName, LeftMouseButtonPressed);

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
        
        _playerAttackStateMachine.SetAttackState(jumpAttackStateName, PlayerVariables.Instance.IS_ATTACKING); //no jump attack

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
            _playerAttackStateMachine.SetAttackState(attackStateName, PlayerAttackState); //toggles state

            timeDifferencebetweenStates = onMouseClickEvent.TimeDifferenceBetweenPressAndRelease();

            _playerAttackStateMachine.TimeDifferenceRequiredBetweenTwoStates(timeDifferenceStateName, timeDifferencebetweenStates);     //keeps track of time elapsed

        }

        if (IsEnumValueEqualToLengthOfEnum<T>(PlayerAttackStateName) || (IsTimeDifferenceWithinRange(timeDifferencebetweenStates, TIME_DIFFERENCE_MAX) && PlayerAttackStateName != _playerAttackStateMachine.GetStateNameThroughEnum(1))) //dont do it for the first attackState
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
        _playerAttackStateMachine.CanAttack(canAttackStateName, PlayerVariables.Instance.IS_ATTACKING);
        _playerAttackStateMachine.CanAttack(jumpAttackStateName, PlayerVariables.Instance.IS_ATTACKING);
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
        bool isDialogueOpen = SceneSingleton.GetDialogueManager().IsOpen();
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = SceneSingleton.GetInventoryOpenCloseManager().isOpenInventory;

        return !isDialogueOpen && !isBuying && !isInventoryOpen && !isJumping;
    }
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

    public void SetMouseClickBeginEndTime(float startTime, float endTime)
    {
        onMouseClickEvent.ClickStartTime = startTime;
        onMouseClickEvent.ClickEndTime = endTime;
    }
    public void InvokeOnMouseClickEvent(float startTime, float endTime)
    {
        onMouseClickEvent.Invoke(startTime, endTime);
    }

}
