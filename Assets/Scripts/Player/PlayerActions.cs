using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngineInternal;

public class PlayerActions : MonoBehaviour, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<GenericStateBundle<GameStateBundle>>, IObserver<CharacterSpeed>, IObserver<CharacterVelocity>
{
    [SerializeField] float _characterSpeed = 10f;

    private PlayerStateDelegator _playerStateDelegator;

    private PlayerStateEvent _playerStateEvent;

    private PlayerInput _playerInput;

    private Rocky2DActions _rocky2DActions;

    private Rigidbody2D _rb;

    private SpriteRenderer _spriteRenderer;

    private Vector2 _keystrokeTrack;

    private bool _daggerInput = false;

    private IReceiverEnhancedAsync<JumpingController, bool> _jumpReceiver;

    private CommandAsyncEnhanced<JumpingController, bool> _jumpCommand;

    private IReceiverEnhancedAsync<SlidingController, bool> _slidingReceiver;

    private CommandAsyncEnhanced<SlidingController, bool> _slidingCommand;

    private IReceiverAsync<bool> _slideReceiver;

    private CommandAsync<bool> _slideCommand;

    private IReceiver<bool> _attackReceiver;

    private Command<bool> _attackCommand;

    private IReceiverEnhancedAsync<PlayerAnimationController, PlayerAnimationControllerPackage<bool>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, PlayerAnimationControllerPackage<bool>> _animationCommand;

    private IReceiver<bool> _throwingProjectileReceiver;

    private Command<bool> _throwingProjectileCommand;

    private PlayerActionsModel _playerActionsModel;

    private GlobalGameStateDelegator _globalGameStateDelegator;

    private FloatDelegator _floatDelegator;

    private PlayerVelocityDelegator _playerVelocityDelegator;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together

    //Force = -2m * sqrt (g * h)
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions

        _spriteRenderer = GetComponent<SpriteRenderer>();

        _playerActionsModel = new PlayerActionsModel();

        _jumpReceiver = GetComponent<IReceiverEnhancedAsync<JumpingController, bool>>();

        _slideReceiver = GetComponent<SlidingController>();

        _attackReceiver = GetComponent<AttackingController>();

        _throwingProjectileReceiver = GetComponent<ThrowingProjectileController>();

        _attackCommand = new Command<bool>(_attackReceiver);

        _animationReceiver = GetComponent<IReceiverEnhancedAsync<PlayerAnimationController, PlayerAnimationControllerPackage<bool>>>();

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, PlayerAnimationControllerPackage<bool>>(_animationReceiver);

        _jumpCommand = new CommandAsyncEnhanced<JumpingController, bool>(_jumpReceiver);

        _slidingReceiver = GetComponent<IReceiverEnhancedAsync<SlidingController, bool>>();

        _slidingCommand = new CommandAsyncEnhanced<SlidingController, bool>(_slidingReceiver);

        _slideCommand = new CommandAsync<bool>(_slideReceiver);

        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _rb = GetComponent<Rigidbody2D>();

        _playerActionsModel.OriginalSpeed = _characterSpeed;

        _globalGameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        _playerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        _floatDelegator = Helper.GetDelegator<FloatDelegator>();

        _playerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();

        _playerStateEvent = Helper.GetCustomEvent<PlayerStateEvent>();

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
        StartCoroutine(_globalGameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(GameStateConsumer).ToString()
        }, CancellationToken.None));

        StartCoroutine(_playerVelocityDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(SlidingController).ToString()
        }, CancellationToken.None));

        StartCoroutine(_playerVelocityDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(JumpingController).ToString()
        }, CancellationToken.None));

        StartCoroutine(_playerStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None));

        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.BoostAttack.Enable();
    }

    private async void Update()
    {
        //make it better - but still this is an improvement, enhancement from singleton
        //more modular

        //think of making it more better
        //make it entirely event based
        if (CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE)) 
        {
            //we'll need to deal with this differently now - this class should not be making the actual animatioon calls, but delegate it appropriately via controllers!!!
            await _animationCommand.Execute(new PlayerAnimationControllerPackage<bool>() { PlayerAnimationExecutionState = PlayerAnimationExecutionState.PLAY_MOVEMENT_ANIMATION, Value = false });
            return;
        }

        //movement
        _keystrokeTrack = await PlayerMovement();

        //Flipping
        if (KeystrokeMagnitudeChecker(_keystrokeTrack))
        {
            if (CurrentPlayerState.StateBundle.PlayerActionState.CurrentState != PlayerActionState.IS_GRABBING)
                FlipCharacter(_keystrokeTrack);
        }

        //jumping
        await _jumpCommand.Execute(_playerActionsModel.GetJumpPressed);

        //ledge grab
        if (CurrentPlayerState.StateBundle.PlayerActionState.CurrentState == PlayerActionState.IS_GRABBING) //tackles the ledgeGrab
        {
            await _slidingCommand.Execute(true);
        }

        //sliding
        if (_playerActionsModel.GetSlidePressed && IsSlidingActionConcluded(CurrentPlayerState.StateBundle))
            _slideCommand.Execute();
        else
            _slideCommand.Cancel();

    }

    #region Controller Mechnism
    private async Task<Vector2> PlayerMovement()
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        if (keystroke.x != 0)
            _playerActionsModel.KeyStrokeDifference = GetKeyStrokeDifference(keystroke);

        _playerActionsModel.CharacterVelocityX = keystroke.x;

        CharacterControllerMove(_playerActionsModel.CharacterVelocityX * _playerActionsModel.CharacterSpeed, _playerActionsModel.CharacterVelocityY);

         await _animationCommand.Execute(new PlayerAnimationControllerPackage<bool>()
        {
            PlayerAnimationExecutionState = PlayerAnimationExecutionState.PLAY_MOVEMENT_ANIMATION,
            Value = keystroke.x == 0 ? false : true
        });
        
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
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState == PlayerAttackState.IS_ATTACKING) ? false : context.ReadValueAsButton();

        //not sliding - see if needs to be in another way later!
        CurrentPlayerState.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_SLIDING, IsConcluded = true };

        _playerStateEvent.Invoke(CurrentPlayerState);
    }
    private void EndSlideAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState == PlayerAttackState.IS_ATTACKING) ? false : context.ReadValueAsButton();
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
        _playerActionsModel.LeftMouseButtonPressed = IsSlidingActionInProgress(CurrentPlayerState.StateBundle) ? false : context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickEnd = (float)context.time;

        AttackingController.InvokeOnMouseClickEvent(_playerActionsModel.TimeForMouseClickStart, _playerActionsModel.TimeForMouseClickEnd);

        _attackCommand.Cancel(_playerActionsModel.LeftMouseButtonPressed);

    }

    private void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = IsSlidingActionInProgress(CurrentPlayerState.StateBundle) ? false : context.ReadValueAsButton();
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


    #region Observer Pattern

    public void OnNotify(CharacterSpeed data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CharacterSpeedHandler(data.Speed);
    }

    public void OnNotify(CharacterVelocity data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        VelocityYEventHandler(data.VelocityY);
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentPlayerState.StateBundle = data.StateBundle;
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState.StateBundle = data.StateBundle;
    }

    #endregion

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

    private bool IsSlidingActionConcluded(PlayerStateBundle playerStateBundle)
    {
        return playerStateBundle.PlayerMovementState.CurrentState == PlayerMovementState.IS_SLIDING &&
               playerStateBundle.PlayerMovementState.IsConcluded;
    }

    private bool IsSlidingActionInProgress(PlayerStateBundle playerStateBundle)
    {
        return playerStateBundle.PlayerMovementState.CurrentState == PlayerMovementState.IS_SLIDING &&
               !playerStateBundle.PlayerMovementState.IsConcluded;
    }

    #endregion
}
