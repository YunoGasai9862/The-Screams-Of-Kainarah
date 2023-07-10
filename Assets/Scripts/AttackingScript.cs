using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackingScript : MonoBehaviour
{
    private Animator _anim;
    private BoxCollider2D col;
    private GameObject dag;
    public static bool canthrowDagger = true;
    private float throwdaggerTime = 0;
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

    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject Dagger;
    [SerializeField] GameObject IceTrail;
    [SerializeField] GameObject IceTrail2;
    [SerializeField] string canAttackStateName;
    [SerializeField] string attackStateName;
    [SerializeField] string timeDifferenceStateName;
    [SerializeField] string jumpAttackStateName;
    [SerializeField] string daggerAttackName;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rocky2DActions = new Rocky2DActions();
        _playerInput = GetComponent<PlayerInput>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        col = GetComponent<BoxCollider2D>();
        _movementHelper = new MovementHelperClass();


        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.Attack.started += PlayerAttackStart;

        _rocky2DActions.PlayerAttack.Attack.canceled += PlayerAttackCancel;

        _rocky2DActions.PlayerAttack.ThrowProjectile.started += Throwdagger;

        _rocky2DActions.PlayerAttack.ThrowProjectile.canceled += Throwdagger;

        _playerAttackState = 0;
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

    private void Throwdagger(InputAction.CallbackContext context)
    {
        throwDagger = context.ReadValueAsButton();
    }

    private void PlayerAttackStart(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();

        if (IsAttackPrerequisiteMet()) //ground attack
        {
            _timeForMouseClickStart = (float)context.time;

            globalVariablesAccess.setAttacking(true);

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
        bool isJumping = globalVariablesAccess.ISJUMPING;
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

            globalVariablesAccess.setAttacking(false); //once the user stops clicking, it should be set to false
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
        _playerAttackState = 0; //resets the attackingstate
        globalVariablesAccess.setAttacking(false);
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
        bool isDialogueOpen = SingletonForObjects.getDialogueManager().getIsOpen();
        bool isJumping = globalVariablesAccess.ISJUMPING;
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = SingletonForObjects.getInventoryOpenCloseManager().isOpenInventory;
        bool isSliding = globalVariablesAccess.ISSLIDING;

        return globalVariablesAccess.boolConditionAndTester(!isDialogueOpen, !isBuying, !isInventoryOpen, !isSliding, !isJumping);

    }
    void AttackingMechanism()
    {

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("ThrowDagger"))
        {
            throwdaggerTime += Time.deltaTime;
            if (throwdaggerTime > .5f)
            {
                _anim.SetBool("ThrowDagger", false);
                throwdaggerTime = 0f;

            }


        }

        if (!(_anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) && Input.GetKeyDown(KeyCode.F) && canthrowDagger)
        {
            _anim.SetBool("ThrowDagger", true);
            AttackEnemy.ThrowDagger = true;

            Invoke("instantiateDag", .4f);
            canthrowDagger = false;


        }

    }

    void instantiateDag()
    {
        Vector3 position = transform.position;
        position.y = transform.position.y - 1f;

        Instantiate(Dagger, position, Quaternion.identity);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DaggerPickUp"))
        {
            canthrowDagger = true;
            CreateInventorySystem.AddToInventory(collision.gameObject.GetComponent<SpriteRenderer>().sprite, collision.gameObject.tag);
            Destroy(collision.gameObject);
        }
    }

    public void Icetail()
    {

        GameObjectInstantiator iceTrail = new GameObjectInstantiator(IceTrail);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.setGameObjectParent(transform);


    }

    public void Icetail2()
    {

        GameObjectInstantiator iceTrail = new GameObjectInstantiator(IceTrail2);
        iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
        iceTrail.setGameObjectParent(transform);


    }
}
