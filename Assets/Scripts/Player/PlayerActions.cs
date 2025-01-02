using System;
using System.Collections.Generic;
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
    private bool _daggerInput = false;
    private IReceiver<bool> _jumpReceiver;
    private Command<bool> _jumpCommand;
    private IReceiverAsync<bool> _slideReceiver;
    private CommandAsync<bool> _slideCommand;
    private IReceiver<bool> _attackReceiver;
    private Command<bool> _attackCommand;
    private IReceiver<bool> _throwingProjectileReceiver;
    private Command<bool> _throwingProjectileCommand;
    private PlayerActionsModel _playerActionsModel;

    [SerializeField] float _characterSpeed = 10f;

    public LedgeGrabController LedgeGrabController { get => GetComponent<LedgeGrabController>(); }
    public SlidingController SlidingController { get => GetComponent<SlidingController>(); }
    public JumpingController JumpingController { get => GetComponent<JumpingController>(); }
    public AttackingController AttackingController { get => GetComponent<AttackingController>(); }
    public ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together


    //Force = -2m * sqrt (g * h)
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _playerActionsModel = new PlayerActionsModel();
        _jumpReceiver = GetComponent<JumpingController>();
        _slideReceiver = GetComponent<SlidingController>();
        _attackReceiver = GetComponent<AttackingController>();
        _throwingProjectileReceiver = GetComponent<ThrowingProjectileController>();

        _attackCommand = new Command<bool>(_attackReceiver);
        _jumpCommand = new Command<bool>(_jumpReceiver);
        _slideCommand = new CommandAsync<bool>(_slideReceiver);
        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _rb = GetComponent<Rigidbody2D>();
        _playerActionsModel.OriginalSpeed = _characterSpeed;

        _rocky2DActions.PlayerMovement.Jump.started += BeginJumpAction; //i can add the same function
        _rocky2DActions.PlayerMovement.Jump.canceled += EndJumpAction;
        _rocky2DActions.PlayerMovement.Slide.started += BeginSlideAction;
        _rocky2DActions.PlayerMovement.Slide.canceled += EndSlideAction;

        _rocky2DActions.PlayerAttack.Attack.started += HandlePlayerAttackStart;
        _rocky2DActions.PlayerAttack.Attack.canceled += HandlePlayerAttackCancel;
        _rocky2DActions.PlayerAttack.ThrowProjectile.started += HandleDaggerInput;
        _rocky2DActions.PlayerAttack.ThrowProjectile.canceled += HandleDaggerInput;

        _rocky2DActions.PlayerAttack.BoostAttack.started += HandleBoostAttackStart;
        _rocky2DActions.PlayerAttack.BoostAttack.canceled += HandleBoostAttackCancel;

    }

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement
        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map
        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();
        _rocky2DActions.PlayerAttack.BoostAttack.Enable();

        JumpingController.onPlayerJumpEvent.AddListener(VelocityYEventHandler);
        SlidingController.onSlideEvent.AddListener(CharacterSpeedHandler);
    }

    private void Update()
    {
        if (!SceneSingleton.IsDialogueTakingPlace)
        {
            //movement
            _keystrokeTrack = PlayerMovement();

            //Flipping
            if (KeystrokeMagnitudeChecker(_keystrokeTrack))
            {
                if (!PlayerVariables.Instance.IS_GRABBING)
                    FlipCharacter(_keystrokeTrack);
            }

            //jumping
            _jumpCommand.Execute(_playerActionsModel.GetJumpPressed);

            //ledge grab
            if (PlayerVariables.Instance.IS_GRABBING) //tackles the ledgeGrab
            {
                LedgeGrabController.PerformLedgeGrab();
            }

            //sliding
            if (_playerActionsModel.GetSlidePressed && !PlayerVariables.Instance.slideVariableEvent.PlayerFinishedSliding)
                _slideCommand.Execute();
            else
                _slideCommand.Cancel();
        }

    }

    #region Controller Mechnism
    private Vector2 PlayerMovement()
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        if (keystroke.x != 0)
            _playerActionsModel.KeyStrokeDifference = GetKeyStrokeDifference(keystroke);

        _playerActionsModel.CharacterVelocityX = keystroke.x;

        CharacterControllerMove(_playerActionsModel.CharacterVelocityX * _playerActionsModel.CharacterSpeed, _playerActionsModel.CharacterVelocityY);

        _animationHandler.RunningWalkingAnimation(keystroke.x);  //for movement, plays the animation

        _playerActionsModel.CharacterSpeed = _playerActionsModel.OriginalSpeed; //reset speed

        return keystroke;
    }

    private float GetKeyStrokeDifference(Vector2 keystroke)
    {
        return Vector2.zero.x + keystroke.x;
    }

    private void VelocityYEventHandler(float characterVelocityY)
    {
        _playerActionsModel.CharacterVelocityY = characterVelocityY;
    }
    private void CharacterSpeedHandler(float characterSpeed)
    {
        _playerActionsModel.CharacterSpeed = characterSpeed;
    }
    private void CharacterControllerMove(float CharacterPositionX, float CharacterPositionY)
    {
        _rb.linearVelocity = new Vector2(CharacterPositionX, CharacterPositionY);
    }

    private bool KeystrokeMagnitudeChecker(Vector2 _keystrokeTrack)
    {
        return _keystrokeTrack.magnitude != 0;
    }

    private void BeginJumpAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetJumpPressed = _playerActionsModel.GetSlidePressed ? false : context.ReadValueAsButton();
    }

    private void EndJumpAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetJumpPressed = _playerActionsModel.GetSlidePressed ? false : context.ReadValueAsButton();
    }

    private void BeginSlideAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || PlayerVariables.Instance.IS_ATTACKING == true) ? false : context.ReadValueAsButton();

        PlayerVariables.Instance.slideVariableEvent.SetPlayerSlideState(false);
    }
    private void EndSlideAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || PlayerVariables.Instance.IS_ATTACKING == true) ? false : context.ReadValueAsButton();
    }

    //attacking mechanism centralized
    private void HandleDaggerInput(InputAction.CallbackContext context)
    {
        _playerActionsModel.DaggerInput = context.ReadValueAsButton();

        ThrowingProjectileController.InvokeThrowableProjectileEvent(_playerActionsModel.DaggerInput);

        _throwingProjectileCommand.Execute();
    }

    private void HandlePlayerAttackCancel(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true) ? false : context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickEnd = (float)context.time;

        AttackingController.InvokeOnMouseClickEvent(_playerActionsModel.TimeForMouseClickStart, _playerActionsModel.TimeForMouseClickEnd);

        _attackCommand.Cancel(_playerActionsModel.LeftMouseButtonPressed);

    }

    private void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = (PlayerVariables.Instance.IS_SLIDING == true) ? false : context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickStart = (float)context.time;

        //send time stamps to the attacking controller
        AttackingController.InvokeOnMouseClickEvent(_playerActionsModel.TimeForMouseClickStart, _playerActionsModel.TimeForMouseClickEnd);

        //execute Attack
        _attackCommand.Execute(_playerActionsModel.LeftMouseButtonPressed);
    }

    //boost v attack
    private void HandleBoostAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.VBoostKeyPressed = context.ReadValueAsButton();
        AttackingController.AlertBoostEventForKeyPressed(_playerActionsModel.VBoostKeyPressed);

    }
    private void HandleBoostAttackCancel(InputAction.CallbackContext context)
    {
        _playerActionsModel.VBoostKeyPressed = context.ReadValueAsButton();
    }

    #endregion

    #region Helper functions
    private void FlipCharacter(Vector2 keystroke)
    {
        if (_playerActionsModel.KeyStrokeDifference == -1 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);

        }
        else if (_playerActionsModel.KeyStrokeDifference == 1 && transform.localScale.x < 0 || _playerActionsModel.KeyStrokeDifference == -1 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    #endregion
}
