using CoreCode;
using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GVA = GlobalAccessAndGameHelper.globalVariablesAccess;
public class AttackingScript : MonoBehaviour
{
    private Animator _anim;
    private CapsuleCollider2D col;
    public static bool canthrowDagger = true;
    private float _timeForMouseClickStart = 0;
    private float _timeForMouseClickEnd = 0;
    private MovementHelperClass _movementHelper;
    private Rocky2DActions _rocky2DActions;
    private PlayerInput _playerInput;
    private bool leftMouseButtonPressed;
    private int _playerAttackState = 0;
    private string _playerAttackStateName;
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

    private void Awake()
    {
        DontDestroyOnLoad(this);

        _anim = GetComponent<Animator>();

        _spriteRenderer = GetComponent<SpriteRenderer>();   

        _rocky2DActions = new Rocky2DActions();

        _playerInput = GetComponent<PlayerInput>();

        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);

        col = GetComponent<CapsuleCollider2D>();

        _movementHelper = new MovementHelperClass();

        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.Attack.started += PlayerAttackStart;

        _rocky2DActions.PlayerAttack.Attack.canceled += PlayerAttackCancel;

        _rocky2DActions.PlayerAttack.ThrowProjectile.started += ThrowdaggerInput;

        _rocky2DActions.PlayerAttack.ThrowProjectile.canceled += ThrowdaggerInput;

        _playerAttackState = 0;
    }

    private void Start()
    {
        _pickableItems = GameObject.FindWithTag(pickableItemClassTag).GetComponent<PickableItemsClass>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAttackPrerequisiteMet())
        {
            if (_playerAttackStateMachine.istheAttackCancelConditionTrue(_playerAttackStateName, Enum.GetNames(typeof(PlayerAttackEnum.PlayerAttackSlash)))) //for the first status only
            {
                ResetAttackStatuses();
            }

        }
    }

    private void ThrowdaggerInput(InputAction.CallbackContext context)
    {
        throwDagger = context.ReadValueAsButton();

        _playerAttackStateMachine.setAttackState(AnimationConstants.THROWDAGGER, throwDagger);

        GameObject daggerInventorySlot = CreateInventorySystem.GetSlotTheGameObjectIsAttachedTo("Dagger");

        if (daggerInventorySlot != null)
        {
            ThrowDagger(_pickableItems.returnGameObjectForTheKey("Dagger"));
        }

    }

    private async void ThrowDagger(GameObject prefab)
    {

        GameObjectInstantiator _daggerInstantiator = new(prefab);

        GameObject _daggerGameObject = _daggerInstantiator.InstantiateGameObject(getDaggerPositionwithOffset(2, -1), Quaternion.identity);

        await CreateInventorySystem.ReduceQuantity(prefab.gameObject.tag);

        _daggerGameObject.GetComponent<AttackEnemy>().throwDagger = true;


    }

    public Vector2 getDaggerPositionwithOffset(float xOffset, float yOffset)
    {
        return isPlayerFlipped(_spriteRenderer)? new Vector2(transform.position.x - xOffset, transform.position.y + yOffset) :
            new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);
    }

    public bool isPlayerFlipped(SpriteRenderer _sr)
    {
        return _sr.flipX;
    }

    private void PlayerAttackStart(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();

        if (IsAttackPrerequisiteMet()) //ground attack
        {
            _timeForMouseClickStart = (float)context.time;

            GVA.setAttacking(true);

            //keeps track of attacking states
            _isPlayerEligibleForStartingAttack = enumStateManipulator<PlayerAttackEnum.PlayerAttackSlash>(ref _playerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

            //sets the initial configuration for the attacking system
            settingInitialAttackConfiguration(canAttackStateName, leftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>();

        }

        if (isJumpAttackPrequisitesMet())
        {
            _playerAttackStateMachine.setAttackState(jumpAttackStateName, leftMouseButtonPressed);

        }
    }

    private bool isJumpAttackPrequisitesMet()
    {
        bool isJumping = GVA.ISJUMPING;
        bool isOnTheGround = _movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground);

        return isJumping && !isOnTheGround;
    }

    private void PlayerAttackCancel(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();

        if (IsAttackPrerequisiteMet())
        {

            _timeForMouseClickEnd = (float)context.time;

            _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

            GVA.setAttacking(false); //once the user stops clicking, it should be set to false
        }

        _playerAttackStateMachine.setAttackState(jumpAttackStateName, leftMouseButtonPressed); //no jump attack

    }
    private bool enumStateManipulator<T>(ref int PlayerAttackState, int InitialStateOfTheEnum)
    {
        int EnumSize = getLenthofEnum<T>(); //returns the length of the Enum

        foreach (T PAS in Enum.GetValues(typeof(T)))
        {
            int _dummy = Convert.ToInt32(PAS) - 1; //converts to INT

            if (PlayerAttackState == _dummy)
            {
                _dummy++;

                PlayerAttackState = _dummy <= EnumSize ? _dummy : InitialStateOfTheEnum; //sets to the initial State of the Enum

                _playerAttackStateName = Enum.GetName(typeof(T), PlayerAttackState);


                return true;
            }
        }

        return false;
    }

    private void settingInitialAttackConfiguration(string canAttackStateName, bool leftMouseButtonPressed)
    {

        _playerAttackStateMachine.canAttack(canAttackStateName, leftMouseButtonPressed);
    }
    private void PlayerAttackMechanism<T>()
    {
        if (_isPlayerEligibleForStartingAttack) //cast Type <T>
        {

            _playerAttackStateMachine.setAttackState(attackStateName, _playerAttackState); //toggles state

            timeDifferencebetweenStates = timeDifference(_timeForMouseClickEnd, _timeForMouseClickStart);

            _playerAttackStateMachine.timeDifferenceRequiredBetweenTwoStates(timeDifferenceStateName,     //keeps track of time elapsed
         timeDifferencebetweenStates);

            if (isEnumValueEqualToLengthOfEnum<T>(_playerAttackStateName) ||
                (isTimeDifferenceWithinRange(timeDifferencebetweenStates, 1.5f) &&
                _playerAttackStateName != _playerAttackStateMachine.getStateNameThroughEnum(1))) //dont do it for the first attackState
            {
                ResetAttackStatuses();
                return;
            }

        }
    }

    private bool isTimeDifferenceWithinRange(float value, float upperBoundary)
    {
        return value > upperBoundary;
    }
    private void ResetAttackStatuses()
    {
        _playerAttackStateMachine.canAttack(canAttackStateName, false);
        _playerAttackStateMachine.canAttack(jumpAttackStateName, false);
        _playerAttackState = 0; //resets the attackingstate
        GVA.setAttacking(false);
    }


    private float timeDifference(float EndTime, float StartTime)
    {
        return Math.Abs(EndTime - StartTime);
    }
    private bool isEnumValueEqualToLengthOfEnum<T>(string _playerAttackStateName)
    {
        return (int)Enum.Parse(typeof(T), _playerAttackStateName) == getLenthofEnum<T>();
    }

    private int getLenthofEnum<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }
    public bool IsAttackPrerequisiteMet()
    {
        bool isDialogueOpen = GameObjectCreator.GetDialogueManager().getIsOpen();
        bool isJumping = GVA.ISJUMPING;
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = GameObjectCreator.GetInventoryOpenCloseManager().isOpenInventory;
        bool isSliding = GVA.ISSLIDING;

        return GVA.boolConditionAndTester(!isDialogueOpen, !isBuying, !isInventoryOpen, !isSliding, !isJumping);

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
}
