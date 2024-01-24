using CoreCode;
using NUnit.Framework.Internal;
using PlayerAnimationHandler;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackingController : MonoBehaviour, IReceiver<bool>
{
    private const float TIME_DIFFERENCE_MAX = 1.5f;

    private Animator _anim;
    private CapsuleCollider2D col;
    public static bool canthrowDagger = true;
    private float _timeForMouseClickStart = 0;
    private float _timeForMouseClickEnd = 0;
    private MovementHelperClass _movementHelper;
    private Rocky2DActions _rocky2DActions;
    private PlayerInput _playerInput;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private bool _isPlayerEligibleForStartingAttack = false;
    private float timeDifferencebetweenStates;
    private bool throwDagger = false;
    private SpriteRenderer _spriteRenderer;
    private PickableItemsClass _pickableItems;

    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject IceTrail;
    [SerializeField] GameObject IceTrail2;
    [SerializeField] string canAttackStateName;
    [SerializeField] string attackStateName;
    [SerializeField] string timeDifferenceStateName;
    [SerializeField] string jumpAttackStateName;
    [SerializeField] string daggerAttackName;
    [SerializeField] string pickableItemClassTag;
    public bool LeftMouseButtonPressed { get; set; }
    private int PlayerAttackState { get; set; }
    private string PlayerAttackStateName { get; set; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        _spriteRenderer = GetComponent<SpriteRenderer>();   

        _rocky2DActions = new Rocky2DActions();

        _playerInput = GetComponent<PlayerInput>();

        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);

        col = GetComponent<CapsuleCollider2D>();

        _movementHelper = new MovementHelperClass();

      // _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        //_rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.Attack.started += HandlePlayerAttackStart;

        _rocky2DActions.PlayerAttack.Attack.canceled += HandlePlayerAttackCancel;

        _rocky2DActions.PlayerAttack.ThrowProjectile.started += ThrowDaggerInput;

        _rocky2DActions.PlayerAttack.ThrowProjectile.canceled += ThrowDaggerInput;

        PlayerAttackState = 0;
    }

    private void Start()
    {
        _pickableItems = GameObject.FindWithTag(pickableItemClassTag).GetComponent<PickableItemsClass>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerVariables.Instance.IS_SLIDING || _playerAttackStateMachine.IstheAttackCancelConditionTrue(PlayerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
        {
            ResetAttackingState();
        }
    }

    private void ThrowDaggerInput(InputAction.CallbackContext context)
    {
        throwDagger = context.ReadValueAsButton();

        _playerAttackStateMachine.SetAttackState(AnimationConstants.THROWDAGGER, throwDagger);

        GameObject daggerInventorySlot = CreateInventorySystem.GetSlotTheGameObjectIsAttachedTo("Dagger");

        if (daggerInventorySlot != null)
        {
            ThrowDagger(_pickableItems.returnGameObjectForTheKey("Dagger"));
        }

    }

    private async void ThrowDagger(GameObject prefab)
    {
        GameObjectInstantiator _daggerInstantiator = new(prefab);

        GameObject _daggerGameObject = _daggerInstantiator.InstantiateGameObject(GetDaggerPositionWithOffset(2, -1), Quaternion.identity);

        await CreateInventorySystem.ReduceQuantity(prefab.gameObject.tag);

        _daggerGameObject.GetComponent<AttackEnemy>().throwDagger = true;
    }

    public Vector2 GetDaggerPositionWithOffset(float xOffset, float yOffset)
    {
        return IsPlayerFlipped(_spriteRenderer)? new Vector2(transform.position.x - xOffset, transform.position.y + yOffset) :
            new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);
    }

    public bool IsPlayerFlipped(SpriteRenderer _sr)
    {
        return _sr.flipX;
    }

    private void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true)? false: context.ReadValueAsButton();

        if (IsAttackPrerequisiteMet()) //ground attack
        {
            _timeForMouseClickStart = (float)context.time;

            PlayerVariables.Instance.attackVariableEvent.Invoke(true);
            //keeps track of attacking state
            (PlayerAttackState, PlayerAttackStateName, _isPlayerEligibleForStartingAttack) = GetEnumStateAndName<PlayerAttackEnum.PlayerAttackSlash>( PlayerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

            //sets the initial configuration for the attacking system
            _playerAttackStateMachine.CanAttack(canAttackStateName, LeftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>(_isPlayerEligibleForStartingAttack);

        }

        if (IsJumpAttackPrequisitesMet())
        {
            _playerAttackStateMachine.SetAttackState(jumpAttackStateName, LeftMouseButtonPressed);

        }
    }

    private bool IsJumpAttackPrequisitesMet()
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnTheGround = _movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground);

        return isJumping && !isOnTheGround;
    }

    private void HandlePlayerAttackCancel(InputAction.CallbackContext context)
    {
        LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true) ? false : context.ReadValueAsButton();

        _timeForMouseClickEnd = (float)context.time;

        _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

        PlayerVariables.Instance.attackVariableEvent.Invoke(false);
        
        _playerAttackStateMachine.SetAttackState(jumpAttackStateName, LeftMouseButtonPressed); //no jump attack

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

            timeDifferencebetweenStates = TimeDifference(_timeForMouseClickEnd, _timeForMouseClickStart);

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

    private float TimeDifference(float EndTime, float StartTime)
    {
        return Math.Abs(EndTime - StartTime);
    }
    private bool IsEnumValueEqualToLengthOfEnum<T>(string _playerAttackStateName)
    {
        return (int)Enum.Parse(typeof(T), _playerAttackStateName) == GetLenthofEnum<T>();
    }

    private int GetLenthofEnum<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }
    public bool IsAttackPrerequisiteMet()
    {
        bool isDialogueOpen = GameObjectCreator.GetDialogueManager().IsOpen();
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = GameObjectCreator.GetInventoryOpenCloseManager().isOpenInventory;

        return !isDialogueOpen && !isBuying && !isInventoryOpen && !isJumping;
    }
    public void Icetail()
    {
        GameObjectInstantiator iceTrail = new(IceTrail);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.setGameObjectParent(transform);
    }

    public void Icetail2()
    {
        GameObjectInstantiator iceTrail = new(IceTrail2);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.setGameObjectParent(transform);
    }

    public bool PerformAction(bool value = false)
    {
        return true;
    }

    public bool CancelAction()
    {
        return true;
    }
}
