using CoreCode;
using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerActions : MonoBehaviour 
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
    private Command<bool> _jumpCommand;
    private Command<bool> _slideCommand;
    private IReceiver<bool> _jumpReceiver;
    private IReceiver<bool> _slideReceiver;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxTimeJump;
    [SerializeField] float slidingSpeed;
    [SerializeField] LedgeGrabController ledgeGrabController;
    [SerializeField] JumpingController jumpingController;
    [SerializeField] SlidingController slidingController;

    private float characterVelocityY;
    private float characterVelocityX;
    private bool _isJumpPressed;
    private PlayerAttackStateMachine _playerAttackStateMachine;

    public bool GetJumpPressed { get => _isJumpPressed; set => _isJumpPressed = value; }

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
        _jumpReceiver = GetComponent<JumpingController>();
        _slideReceiver = GetComponent<SlidingController>();
        _jumpCommand = new Command<bool>(_jumpReceiver);
        _slideCommand = new Command<bool>(_slideReceiver);
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
  
    private void Update()
    {
        if (!GameObjectCreator.GetDialogueManager().getIsOpen())
        {
            //movement
            _keystrokeTrack = PlayerMovement();

            //Flipping
            if (KeystrokeMagnitudeChecker(_keystrokeTrack))
                FlipCharacter(_keystrokeTrack, ref _spriteRenderer);

            //jumpining
            _jumpCommand.Execute(GetJumpPressed);

            //ledge grab
            if (PlayerMovementGlobalVariables.ISGRABBING) //tackles the ledgeGrab
            {
                ledgeGrabController.PerformLedgeGrab();
                return;
            }

            //sliding

            if (PlayerMovementHelperFunctions.boolConditionAndTester(PlayerMovementGlobalVariables.ISSLIDING, !PlayerMovementGlobalVariables.ISATTACKING,
                 _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, groundLayer),
                 !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>()))
            {
                _playerAttackStateMachine.ForceDisableAttacking(1);
                CharacterControllerMove(characterVelocityX * slidingSpeed, characterVelocityY);

            }

            if (_animationHandler.returnCurrentAnimation() > .6f && _animationHandler.isNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
            {
                PlayerMovementHelperFunctions.setSliding(false);
            }
        }

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

    private bool KeystrokeMagnitudeChecker(Vector2 _keystrokeTrack)
    {
        return _keystrokeTrack.magnitude != 0;
    }

    private bool FlipCharacter(Vector2 keystroke, ref SpriteRenderer _sr)
    {
        return keystroke.x >= 0 ? _sr.flipX = false : _sr.flipX = true; //flips the character
    }

    private void Jump(InputAction.CallbackContext context)
    {
        GetJumpPressed = context.ReadValueAsButton();
    }

    private void Slide(InputAction.CallbackContext context)
    {
        if (PlayerMovementHelperFunctions.boolConditionAndTester(!GetJumpPressed,
            !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())) //conditions for sliding
        {
            PlayerMovementGlobalVariables.ISSLIDING = context.ReadValueAsButton();

            PlayerMovementHelperFunctions.setAttacking(false); //for some minor fixes

            _animationHandler.Sliding(PlayerMovementGlobalVariables.ISSLIDING);
        }
    }
}
