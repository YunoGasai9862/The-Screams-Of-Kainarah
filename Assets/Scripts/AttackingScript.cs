using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingScript : MonoBehaviour
{
    private Animator _anim;
    private float elapsedTime = 0;
    private bool kickoffElapsedTime;
    private int AttackCount = 0;
    private BoxCollider2D col;
    private GameObject dag;
    public static bool canthrowDagger = true;
    private float throwdaggerTime = 0;
    private float _timeForMouseClickStart = 0;
    private float _timeForMouseClickEnd = 0;
    private MovementHelperClass _movementHelper;
    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject Dagger;
    [SerializeField] GameObject IceTrail;
    [SerializeField] GameObject IceTrail2;
    [SerializeField] string canAttackStateName;
    [SerializeField] string attackStateName;
    [SerializeField] string timeDifferenceStateName;
    [SerializeField] string jumpAttackStateName;

    private Rocky2DActions _rocky2DActions;
    private PlayerInput _playerInput;
    private bool leftMouseButtonPressed;
    private int _playerAttackState = 0;
    private string _playerAttackStateName;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private bool _isPlayerEligibleForStartingAttack = false;
    private float timeDifferencebetweenStates;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rocky2DActions = new Rocky2DActions();
        _playerInput = GetComponent<PlayerInput>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);


        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map
        _rocky2DActions.PlayerAttack.Attack.started += PlayerAttackStart;
        _rocky2DActions.PlayerAttack.Attack.canceled += PlayerAttackCancel;

        _playerAttackState = 0;
    }


    private void PlayerAttackStart(InputAction.CallbackContext context)
    {
        if (IsAttackPrerequisiteMet())
        {
            leftMouseButtonPressed = context.ReadValueAsButton();

            _timeForMouseClickStart = (float)context.time;

            globalVariablesAccess.ISATTACKING = true;

            //keeps track of attacking states
            _isPlayerEligibleForStartingAttack = enumStateManipulator<PlayerAttackEnum.PlayerAttackSlash>(ref _playerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

            //sets the initial configuration for the attacking system
            settingInitialAttackConfiguration(canAttackStateName, leftMouseButtonPressed);

            PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>();
        }

    }
    private void jumpAttackMechanism()
    {

        if (isJumpPrequisitesMet())
        {
            _playerAttackStateMachine.setAttackState(jumpAttackStateName, true);
        }

    }

    private bool isJumpPrequisitesMet()
    {
        bool isJumping = globalVariablesAccess.ISJUMPING;
        bool isOnTheGround = _movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground);

        return isJumping && !isOnTheGround;
    }

    private void PlayerAttackCancel(InputAction.CallbackContext context)
    {
        if (IsAttackPrerequisiteMet())
        {
            leftMouseButtonPressed = context.ReadValueAsButton();

            _timeForMouseClickEnd = (float)context.time;

            _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

            globalVariablesAccess.ISATTACKING = false; //once the user stops clicking, it should be set to false
        }

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

            if (isTimeDifferenceWithinRange(timeDifferencebetweenStates, 1.5f))
            {
                ResetAttackStatuses();
                return;
            }


        }

        if (isEnumValueEqualToLengthOfEnum<T>(_playerAttackStateName))
        {
            ResetAttackStatuses();
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
        globalVariablesAccess.ISATTACKING = false;
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

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        _movementHelper = new MovementHelperClass();

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

    public bool IsAttackPrerequisiteMet()
    {
        bool isDialogueOpen = SingletonForObjects.getDialogueManager().getIsOpen();
        bool isBuying = OpenWares.Buying;
        bool isInventoryOpen = SingletonForObjects.getInventoryOpenCloseManager().isOpenInventory;
        bool isSliding = globalVariablesAccess.ISSLIDING;

        return globalVariablesAccess.boolConditionAndTester(!isDialogueOpen, !isBuying, !isInventoryOpen, !isSliding);

    }
    void AttackingMechanism()
    {

        if (!_movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground) && Input.GetMouseButtonDown(0))
        {
            _anim.SetBool("AttackJ", true);
        }
        else
        {
            _anim.SetBool("AttackJ", false);

        }

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

        GameObject Ice = Instantiate(IceTrail, transform.position, Quaternion.identity);
        Ice.transform.parent = transform;


    }

    public void Icetail2()
    {

        GameObject Ice = Instantiate(IceTrail2, transform.position, Quaternion.identity);
        Ice.transform.parent = transform;

    }
}
