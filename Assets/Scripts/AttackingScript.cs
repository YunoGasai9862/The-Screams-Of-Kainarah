using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingScript : MonoBehaviour
{
    private Animator anim;
    private float elapsedTime = 0;
    private bool kickoffElapsedTime;
    private int AttackCount = 0;
    private BoxCollider2D col;
    private GameObject dag;
    public static bool canthrowDagger = true;
    private float throwdaggerTime = 0;
    private double _timeForMouseClickStart = 0;
    private double _timeForMouseClickEnd = 0;
    private MovementHelperClass _movementHelper;
    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject Dagger;
    [SerializeField] GameObject IceTrail;
    [SerializeField] GameObject IceTrail2;


    private Rocky2DActions _rocky2DActions;
    private PlayerInput _playerInput;
    private bool leftMouseButtonPressed;
    private int _playerAttackState = 0;

    private void Awake()
    {
        _rocky2DActions = new Rocky2DActions();
        _playerInput = GetComponent<PlayerInput>();


        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map
        _rocky2DActions.PlayerAttack.Attack.started += PlayerAttackStart;
        _rocky2DActions.PlayerAttack.Attack.canceled += PlayerAttackCancel;

        _playerAttackState = (int)PlayerAttackEnum.PlayerAttackSlash.Attack; //always the first attack
    }


    private void PlayerAttackStart(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();
        _timeForMouseClickStart = context.time;

    }

    private void PlayerAttackCancel(InputAction.CallbackContext context)
    {
        leftMouseButtonPressed = context.ReadValueAsButton();
        _timeForMouseClickEnd = context.time;


    }

    private bool enumStateManipulator<T>(ref int PlayerAttackState, int InitialStateOfTheEnum)
    {
        int EnumSize = Enum.GetNames(typeof(T)).Length; //returns the length of the Enum
        foreach (T PAS in Enum.GetValues(typeof(T)))
        {
            int _dummy = Convert.ToInt32(PAS); //converts to INT
            if (PlayerAttackState == _dummy)
            {
                _dummy++;
                PlayerAttackState = _dummy <= EnumSize ? _dummy : InitialStateOfTheEnum; //sets to the initial State of the Enum
                return true;
            }
        }

        return false;
    }

    private void PlayerAttackMechanism()
    {
        if (enumStateManipulator<PlayerAttackEnum.PlayerAttackSlash>(ref _playerAttackState, (int)PlayerAttackEnum.PlayerAttackSlash.Attack)) //cast Type <T>
        {

        }
    }


    private double timeDifference(double EndTime, double StartTime)
    {
        return EndTime - StartTime;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
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
            anim.SetBool("AttackJ", true);
        }
        else
        {
            anim.SetBool("AttackJ", false);

        }

        if ((_movementHelper.overlapAgainstLayerMaskChecker(ref col, Ground) ||
            _movementHelper.overlapAgainstLayerMaskChecker(ref col, ledge)) && Input.GetMouseButtonDown(0))
        {
            kickoffElapsedTime = true;

            AttackCount++;
            anim.SetInteger("PlayerAttack", AttackCount);

            anim.SetBool("Attack", true);
            elapsedTime = 0;  // YAYAY SOLVED IT!!!

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //fix with new elapsedTime thingy

            anim.SetFloat("ElapsedTime", elapsedTime);

            if (elapsedTime > .5f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }


        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }



        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }

        if (AttackCount > 4)
        {
            anim.SetBool("Attack", false);
            AttackCount = 0;

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ThrowDagger"))
        {
            throwdaggerTime += Time.deltaTime;
            if (throwdaggerTime > .5f)
            {
                anim.SetBool("ThrowDagger", false);
                throwdaggerTime = 0f;

            }


        }

        if (!(anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) && Input.GetKeyDown(KeyCode.F) && canthrowDagger)
        {
            anim.SetBool("ThrowDagger", true);
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
