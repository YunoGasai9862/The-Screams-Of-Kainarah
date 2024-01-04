using CoreCode;
using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerActions : MonoBehaviour //why i removed the MonoBehavior? The notified subject class inherits from MonoBehavior so does the PlayerActions now, but also 
                                           //have the ability to inherit notifiy actions
{
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerAnimationMethods _animationHandler;
    private Vector2 _keystrokeTrack;
    private IOverlapChecker _movementHelperClass;
    private CapsuleCollider2D _capsulecollider;
    private Animator _anim;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxTimeJump;
    [SerializeField] float slidingSpeed;
    [SerializeField] LedgeGrabController ledgeGrabController;

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
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = new MovementHelperClass();
        _capsulecollider = GetComponent<CapsuleCollider2D>();
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
        if (canPlayerJump()) //jumping
        {
            globalVariablesAccess.ISJUMPING = true;

            characterVelocityY = JumpSpeed * .5f;

            _animationHandler.JumpingFalling(globalVariablesAccess.ISJUMPING); //jumping animation


        }

        if (canPlayerFall() || MaxJumpTimeChecker()) //peak reached
        {
            globalVariablesAccess.ISJUMPING = false;

            characterVelocityY = -JumpSpeed * .8f;

            isJumpPressed = false; //fixed the issue of eternally looping at jumep on JUMP HOLD

        }


        if (!globalVariablesAccess.ISJUMPING && !LedgeGroundChecker(groundLayer, ledgeLayer)) //falling
        {
            characterVelocityY = -JumpSpeed * .8f; //extra check
            _animationHandler.JumpingFalling(globalVariablesAccess.ISJUMPING); //falling naimation

        }

        if (_rb.velocity.y > 0f) //how high the player can jump
        {
            _timeCounter += Time.deltaTime;
        }

        if (PlayerActionRelayer.isGrabbing) //tackles the ledgeGrab
        {
            HandleIsGrabbingScenario();
            ledgeGrabController.PerformLedgeGrab();
            return;
        }


        if (!globalVariablesAccess.ISJUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && !isJumpPressed) //on the ground
        {
            characterVelocityY = 0f;
            _timeCounter = 0;
        }

    }

    private bool canPlayerJump()
    {
        bool _isJumping = globalVariablesAccess.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return globalVariablesAccess.boolConditionAndTester(!_isJumping, _isOntheLedgeOrGround, _isJumpPressed);
    }

    private bool canPlayerFall()
    {
        bool _isJumping = globalVariablesAccess.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return globalVariablesAccess.boolConditionAndTester(_isJumping, !_isOntheLedgeOrGround, !_isJumpPressed);
    }

    private bool LedgeGroundChecker(LayerMask ground, LayerMask ledge)
    {
        return _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, ground)
            || _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, ledge);
    }

    private void HandleIsGrabbingScenario()
    {
        _rb.velocity = new Vector2(0, 0);
        _rb.gravityScale = 0;

    }

    private void Update()
    {
        if (!GameObjectCreator.GetDialogueManager().getIsOpen())
        {
            //movement
            _keystrokeTrack = PlayerMovement();

            //Flipping
            if (keystrokeMagnitudeChecker(_keystrokeTrack))
                FlipCharacter(_keystrokeTrack, ref _spriteRenderer);
            //jumpining

            HandleJumping();

            //sliding

            if (globalVariablesAccess.boolConditionAndTester(globalVariablesAccess.ISSLIDING, !globalVariablesAccess.ISATTACKING,
                 _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, groundLayer),
                 !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>()))
            {
                _playerAttackStateMachine.ForceDisableAttacking(1);
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
    }

    private void Slide(InputAction.CallbackContext context)
    {
        if (globalVariablesAccess.boolConditionAndTester(!getJumpRessed(),
            !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())) //conditions for sliding
        {
            globalVariablesAccess.ISSLIDING = context.ReadValueAsButton();

            globalVariablesAccess.setAttacking(false); //for some minor fixes

            _animationHandler.Sliding(globalVariablesAccess.ISSLIDING);
        }
    }

    private bool getJumpRessed()
    {
        return isJumpPressed;
    }


}
