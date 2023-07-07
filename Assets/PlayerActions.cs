using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private AnimationHandler _animationHandler;
    private Vector2 _keystrokeTrack;
    private IOverlapChecker _movementHelperClass;
    private BoxCollider2D _boxCollider;
    private Animator _anim;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxTimeJump;
    [SerializeField] float slidingSpeed;

    private float characterVelocityY;
    private float characterVelocityX;
    private bool isJumpPressed;
    private float _timeCounter;
    private PlayerAttackStateMachine _playerAttackStateMachine;

    //Force = -2m * sqrt (g * h)

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _anim = GetComponent<Animator>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<AnimationHandler>();
        _movementHelperClass = new MovementHelperClass();
        _boxCollider = GetComponent<BoxCollider2D>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        //have the actionMappings
        _rb = GetComponent<Rigidbody2D>();

        _rocky2DActions.PlayerMovement.Jump.started += Jump; //i can add the same function
        _rocky2DActions.PlayerMovement.Jump.canceled += Jump;
        _rocky2DActions.PlayerMovement.Slide.started += Slide;
        _rocky2DActions.PlayerMovement.Slide.canceled += Slide;



    }


    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

    }

    public void HandleJumping()
    {

        if (!globalVariablesAccess.ISJUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && isJumpPressed)
        {
            globalVariablesAccess.ISJUMPING = true;
            characterVelocityY = JumpSpeed * .5f;

        }

        if ((globalVariablesAccess.ISJUMPING && !(LedgeGroundChecker(groundLayer, ledgeLayer)) && !isJumpPressed) || MaxJumpTimeChecker())
        {
            globalVariablesAccess.ISJUMPING = false;
            characterVelocityY = -JumpSpeed * .8f;

        }

        if (!globalVariablesAccess.ISJUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && !isJumpPressed)
        {
            characterVelocityY = 0f;
            _timeCounter = 0;
        }

        if (_rb.velocity.y > 0f)
        {
            _timeCounter += Time.deltaTime;
        }

        if (!globalVariablesAccess.ISJUMPING && !_movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, groundLayer))
        {
            characterVelocityY = -JumpSpeed * .8f; //extra check

        }

    }

    private bool LedgeGroundChecker(LayerMask ground, LayerMask ledge)
    {
        return (_movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, ground)
            || _movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, ledge));
    }


    private void Update()
    {
        if (!SingletonForObjects.getDialogueManager().getIsOpen())
        {
            //movement
            _keystrokeTrack = PlayerMovement();

            //Flipping
            if (keystrokeMagnitudeChecker(_keystrokeTrack))
                FlipCharacter(_keystrokeTrack, ref _spriteRenderer);
            //jumpining

            HandleJumping();

            //sliding

            if (globalVariablesAccess.boolConditionAndTester(globalVariablesAccess.ISSLIDING, !globalVariablesAccess.ISATTACKING) &&
                _movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, groundLayer))
            {
                ForceDisableAttacking();
                CharacterControllerMove(characterVelocityX * slidingSpeed, characterVelocityY);

            }

            if (_animationHandler.returnCurrentAnimation() > .6f && _animationHandler.isNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
            {
                globalVariablesAccess.setSliding(false);
            }


        }

    }


    public bool MaxJumpTimeChecker()
    {
        return _timeCounter > maxTimeJump;
    }
    private Vector2 PlayerMovement()
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        characterVelocityX = keystroke.x;

        CharacterControllerMove(characterVelocityX * _characterSpeed, characterVelocityY);

        _animationHandler.RunningWalkingAnimation(keystroke.x);  //for movement, plays the animation


        return keystroke;
    }

    private void CharacterControllerMove(float CharacterPositionX, float CharacterPositionY)
    {

        _rb.velocity = new Vector2(CharacterPositionX, CharacterPositionY);

    }


    private bool keystrokeMagnitudeChecker(Vector2 _keystrokeTrack)
    {
        return _keystrokeTrack.magnitude != 0;
    }

    private bool FlipCharacter(Vector2 keystroke, ref SpriteRenderer _sr)
    {
        return keystroke.x >= 0 ? _sr.flipX = false : _sr.flipX = true; //flips the character

    }

    private void Jump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();

        _animationHandler.JumpingFalling(isJumpPressed);

    }

    private void Slide(InputAction.CallbackContext context)
    {
        globalVariablesAccess.ISSLIDING = context.ReadValueAsButton();

        if (!getJumpRessed())
        {
            globalVariablesAccess.setAttacking(false); //for some minor fixes
            _animationHandler.Sliding(globalVariablesAccess.ISSLIDING);
        }
    }

    private bool getJumpRessed()
    {
        return isJumpPressed;
    }

    private void ForceDisableAttacking()
    {
        string stateName = _playerAttackStateMachine.getStateNameThroughEnum(1);
        _playerAttackStateMachine.setAttackState(stateName, false);
    }


}
