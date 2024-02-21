using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerActions : MonoBehaviour
{
    private const float MAX_SLIDING_TIME_ALLOW = 0.5f;
    private const float MAX_JUMP_TIME = 0.3f;
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerAnimationMethods _animationHandler;
    private Vector2 _keystrokeTrack;
    private float _timeForMouseClickStart=0f;
    private float _timeForMouseClickEnd=0f;
    private bool _daggerInput = false;


    private IReceiver<bool> _jumpReceiver;
    private Command<bool> _jumpCommand;
    private IReceiverAsync<bool> _slideReceiver;
    private CommandAsync<bool> _slideCommand;
    private IReceiver<bool> _attackReceiver;
    private Command<bool> _attackCommand;
    private IReceiver<bool> _throwingProjectileReceiver;
    private Command<bool> _throwingProjectileCommand;

    [SerializeField] float _characterSpeed = 10f;

    public LedgeGrabController LedgeGrabController { get => GetComponent<LedgeGrabController>(); }
    public SlidingController SlidingController { get => GetComponent<SlidingController>(); }
    public JumpingController JumpingController { get => GetComponent<JumpingController>(); }
    public AttackingController AttackingController { get => GetComponent<AttackingController>(); } 
    public ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together

    private bool GetJumpPressed { get; set; }
    private bool GetSlidePressed { get; set; }
    private float CharacterVelocityY { get; set; }
    private float CharacterVelocityX { get; set; }
    private float CharacterSpeed { get; set; }
    private float SlidingTimeBegin { get; set; }
    private float SlidingTimeEnd { get; set; }
    private float OriginalSpeed { get; set; }
    private bool LeftMouseButtonPressed { get; set; }
    private float TimeForMouseClickStart { get => _timeForMouseClickStart; set => _timeForMouseClickStart = value; }
    private float TimeForMouseClickEnd { get => _timeForMouseClickEnd; set => _timeForMouseClickEnd = value; }
    private bool DaggerInput { get => _daggerInput; set => _daggerInput = value; }
    //Force = -2m * sqrt (g * h)
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();

        _jumpReceiver = GetComponent<JumpingController>();
        _slideReceiver = GetComponent<SlidingController>();
        _attackReceiver = GetComponent<AttackingController>();
        _throwingProjectileReceiver = GetComponent<ThrowingProjectileController>();

        _attackCommand = new Command<bool>(_attackReceiver);
        _jumpCommand = new Command<bool>(_jumpReceiver);
        _slideCommand = new CommandAsync<bool>(_slideReceiver);
        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _rb = GetComponent<Rigidbody2D>();
        OriginalSpeed = _characterSpeed;

        _rocky2DActions.PlayerMovement.Jump.started += BeginJumpAction; //i can add the same function
        _rocky2DActions.PlayerMovement.Jump.canceled += EndJumpAction;
        _rocky2DActions.PlayerMovement.Slide.started += BeginSlideAction;
        _rocky2DActions.PlayerMovement.Slide.canceled += EndSlideAction;

        _rocky2DActions.PlayerAttack.Attack.started += HandlePlayerAttackStart;
        _rocky2DActions.PlayerAttack.Attack.canceled += HandlePlayerAttackCancel;
        _rocky2DActions.PlayerAttack.ThrowProjectile.started += HandleDaggerInput;
        _rocky2DActions.PlayerAttack.ThrowProjectile.canceled += HandleDaggerInput;


    }
    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement
        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map
        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

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
            {
                if(!PlayerVariables.Instance.IS_GRABBING)
                    FlipCharacter(_keystrokeTrack, ref _spriteRenderer);
            }

            //jumping
            _jumpCommand.Execute(GetJumpPressed);

            //ledge grab
            if (PlayerVariables.Instance.IS_GRABBING) //tackles the ledgeGrab
            {
                LedgeGrabController.PerformLedgeGrab();
            }

            //sliding
            if(GetSlidePressed)
                _slideCommand.Execute();
            else
                _slideCommand.Cancel();
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

    private void BeginJumpAction(InputAction.CallbackContext context)
    {
        GetJumpPressed = GetSlidePressed ? false : context.ReadValueAsButton();
    }

    private void EndJumpAction(InputAction.CallbackContext context)
    {
        GetJumpPressed = GetSlidePressed? false : context.ReadValueAsButton();
    }

    private void BeginSlideAction(InputAction.CallbackContext context)
    {
        GetSlidePressed = (GetJumpPressed == true || PlayerVariables.Instance.IS_ATTACKING == true) ? false : context.ReadValueAsButton();
        SlidingTimeBegin = (float)context.time;
    }
    private void EndSlideAction(InputAction.CallbackContext context)
    {
        GetSlidePressed = (GetJumpPressed == true || PlayerVariables.Instance.IS_ATTACKING == true) ? false : context.ReadValueAsButton();
        SlidingTimeEnd = (float)context.time;
    }

    //implement boost feature with slide
    //To-Do


    //attacking mechanism centralized
    private void HandleDaggerInput(InputAction.CallbackContext context)
    {
        DaggerInput = context.ReadValueAsButton();
        ThrowingProjectileController.InvokeThrowableProjectileEvent(DaggerInput);

        _throwingProjectileCommand.Execute();
    }

    private void HandlePlayerAttackCancel(InputAction.CallbackContext context)
    {
        LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true) ? false : context.ReadValueAsButton();
        TimeForMouseClickEnd = (float)context.time;

        AttackingController.InvokeOnMouseClickEvent(TimeForMouseClickStart, TimeForMouseClickEnd);

        _attackCommand.Cancel(LeftMouseButtonPressed);

    }

    private void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true) ? false : context.ReadValueAsButton();
        TimeForMouseClickStart = (float)context.time;

        //send time stamps to the attacking controller
        AttackingController.InvokeOnMouseClickEvent(TimeForMouseClickStart, TimeForMouseClickEnd);

        //execute Attack
        _attackCommand.Execute(LeftMouseButtonPressed);
    }

}
