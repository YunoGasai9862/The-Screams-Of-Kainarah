using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerActions : MonoBehaviour 
{
    private const float MAX_SLIDING_TIME_ALLOW = 0.5f;
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
    public LedgeGrabController LedgeGrabController { get => GetComponent<LedgeGrabController>(); }
    public SlidingController SlidingController { get => GetComponent<SlidingController>(); }
    public JumpingController JumpingController { get => GetComponent<JumpingController>(); }

    private bool GetJumpPressed { get; set; }
    private bool GetSlidePressed { get; set; }
    private float CharacterVelocityY { get; set; }
    private float CharacterVelocityX { get; set; }
    private float CharacterSpeed { get; set; }
    private float SlidingTimeBegin { get; set; }
    private float SlidingTimeEnd { get; set; }
    private float OriginalSpeed { get; set; }

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
        OriginalSpeed = _characterSpeed;

        _rocky2DActions.PlayerMovement.Jump.started += Jump; //i can add the same function
        _rocky2DActions.PlayerMovement.Jump.canceled += Jump;
        _rocky2DActions.PlayerMovement.Slide.started += BeginSlideAction;
        _rocky2DActions.PlayerMovement.Slide.canceled += EndSlideAction;
    }

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        //event subscription
        JumpingController.onPlayerJumpEvent.AddListener(VelocityYEventHandler);
        SlidingController.onSlideEvent.AddListener(CharacterSpeedHandler);
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
            if (PlayerVariables.Instance.IS_GRABBING) //tackles the ledgeGrab
            {
                LedgeGrabController.PerformLedgeGrab();
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

        CharacterSpeed = OriginalSpeed; //reset speed

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

    private void BeginSlideAction(InputAction.CallbackContext context)
    {
        GetSlidePressed = (GetJumpPressed || PlayerVariables.Instance.IS_ATTACKING)? false : context.ReadValueAsButton();
        SlidingTimeBegin = (float)context.time;
    }
    private void EndSlideAction(InputAction.CallbackContext context)
    {
        SlidingTimeEnd = (float)context.time;
    }

    //implement boost feature with slide


}
