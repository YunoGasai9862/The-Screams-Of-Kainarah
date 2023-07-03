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

    private Rocky2DActions _rocky2DActions;
    private PlayerInput _playerInput;
    private bool leftMouseButtonPressed;
    private int _playerAttackState = 0;
    private string _playerAttackStateName;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private bool _isPlayerEligibleForStartingAttack = false;

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
        leftMouseButtonPressed = context.ReadValueAsButton();

        _timeForMouseClickStart = (float)context.time;

        //keeps track of attacking states
        _isPlayerEligibleForStartingAttack = enumStateManipulator<PlayerAttackEnum.PlayerAttackSlash>(ref _playerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack);

        //sets the initial configuration for the attacking system
        settingInitialAttackConfiguration(canAttackStateName, leftMouseButtonPressed);

        PlayerAttackMechanism<PlayerAttackEnum.PlayerAttackSlash>();


    }

    private void PlayerAttackCancel(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();

        _timeForMouseClickEnd = (float)context.time;

        _isPlayerEligibleForStartingAttack = false; //stops so not to create an endless cycle

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
        if (_playerAttackState == (int)PlayerAttackEnum.PlayerAttackSlash.Attack)
        {
            _playerAttackStateMachine.canAttack(canAttackStateName, leftMouseButtonPressed);

        }
    }
    private void PlayerAttackMechanism<T>()
    {
        if (_isPlayerEligibleForStartingAttack) //cast Type <T>
        {
            _playerAttackStateMachine.setAttackState(attackStateName, _playerAttackState); //toggles state

            float timeDifferencebetweenStates = timeDifference(_timeForMouseClickEnd, _timeForMouseClickStart);

            _playerAttackStateMachine.timeDifferenceRequiredBetweenTwoStates(timeDifferenceStateName,     //keeps track of time elapsed
               timeDifferencebetweenStates);
        }

        if (isEnumValueEqualToLengthOfEnum<T>(_playerAttackStateName))
        {
            _playerAttackStateMachine.canAttack(canAttackStateName, false);
            _playerAttackState = 0; //resets the attackingstate
        }


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
        if (PrerequisitesChecker())
        {
            if (kickoffElapsedTime)
            {
                elapsedTime += Time.deltaTime;
            }

            AttackingMechanism();
        }

    }

    public bool PrerequisitesChecker()
    {
        return !SingletonForObjects.getDialogueManager().getIsOpen() &&
            !OpenWares.Buying && !SingletonForObjects.getInventoryOpenCloseManager().isOpenInventory;
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

        if ((_movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground) ||
            _movementHelper.overlapAgainstLayerMaskChecker(ref col, ledge)) && Input.GetMouseButtonDown(0))
        {
            kickoffElapsedTime = true;

            AttackCount++;
            _anim.SetInteger("PlayerAttack", AttackCount);

            _anim.SetBool("Attack", true);
            elapsedTime = 0;  // YAYAY SOLVED IT!!!

        }
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //fix with new elapsedTime thingy

            _anim.SetFloat("ElapsedTime", elapsedTime);

            if (elapsedTime > .5f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                _anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }


        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            _anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                _anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            _anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                _anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }



        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            _anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                _anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }

        if (AttackCount > 4)
        {
            _anim.SetBool("Attack", false);
            AttackCount = 0;

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
