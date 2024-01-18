using GlobalAccessAndGameHelper;
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
    private Command<bool> _jumpCommand;
    private CommandAsync<bool> _slideCommand;
    private IReceiver<bool> _jumpReceiver;
    private IReceiverAsync<bool> _slideReceiver;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LedgeGrabController ledgeGrabController;
    [SerializeField] JumpingController jumpingController;
    [SerializeField] SlidingController slidingController;

    private float _characterVelocityY;
    private float _characterVelocityX;
    private bool _isJumpPressed;
    private bool _isSlidePressed;
    private float _originalSpeed;
    public bool GetJumpPressed { get => _isJumpPressed; set => _isJumpPressed = value; }
    public bool GetSlidePressed { get => _isSlidePressed; set => _isSlidePressed = value; }
    public float CharacterVelocityY { get => _characterVelocityY; set => _characterVelocityY = value; }
    public float CharacterVelocityX { get => _characterVelocityX; set => _characterVelocityX = value; }
    public float CharacterSpeed { get => _characterSpeed; set => _characterSpeed = value; }


    //Force = -2m * sqrt (g * h)

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _jumpReceiver = GetComponent<JumpingController>();
        _slideReceiver = GetComponent<SlidingController>();
        _jumpCommand = new Command<bool>(_jumpReceiver);
        _slideCommand = new CommandAsync<bool>(_slideReceiver);
        _rb = GetComponent<Rigidbody2D>();
        _originalSpeed = _characterSpeed;

        _rocky2DActions.PlayerMovement.Jump.started += Jump; //i can add the same function
        _rocky2DActions.PlayerMovement.Jump.canceled += Jump;
        _rocky2DActions.PlayerMovement.Slide.started += Slide;
        _rocky2DActions.PlayerMovement.Slide.canceled += Slide;

    }

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        //event subscription
        jumpingController.onPlayerJumpEvent.AddListener(VelocityYEventHandler);
        slidingController.onSlideEvent.AddListener(CharacterSpeedHandler);
    }
  
    private void Update()
    {
        if (!GameObjectCreator.GetDialogueManager().IsOpen())
        {
            //movement
            _keystrokeTrack = PlayerMovement();

            //Flipping
            if (KeystrokeMagnitudeChecker(_keystrokeTrack))
                FlipCharacter(_keystrokeTrack, ref _spriteRenderer);

            //jumping
            _jumpCommand.Execute(GetJumpPressed);

            //ledge grab
            if (PlayerVariables.IS_GRABBING) //tackles the ledgeGrab
            {
                ledgeGrabController.PerformLedgeGrab();
                return;
            }

            //sliding
            _slideCommand.Execute(GetSlidePressed);

        }

    }
    private Vector2 PlayerMovement()
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        CharacterVelocityX =  keystroke.x;

        CharacterControllerMove(CharacterVelocityX * CharacterSpeed, CharacterVelocityY);

        _animationHandler.RunningWalkingAnimation(keystroke.x);  //for movement, plays the animation

        CharacterSpeed = _originalSpeed; //reset speed

        return keystroke;
    }
    
    private void VelocityYEventHandler(float characterVelocityY)
    {
        CharacterVelocityY = characterVelocityY;
    }
    private void CharacterSpeedHandler(float characterSpeed)
    {
        CharacterSpeed = characterSpeed;
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
        GetJumpPressed = GetSlidePressed? false: context.ReadValueAsButton();
    }

    private void Slide(InputAction.CallbackContext context)
    {
        GetSlidePressed =  GetJumpPressed? false : context.ReadValueAsButton();
    }
}
