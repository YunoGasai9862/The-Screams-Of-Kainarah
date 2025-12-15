using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<Player>, IObserver<GenericStateBundle<GameStateBundle>>, IObserver<CharacterVelocity>, IDelegate
{
    [SerializeField] float _characterSpeed = 10f;

    private PlayerStateEvent _playerStateEvent;

    private Rocky2DActions _rocky2DActions;

    private Player Player { get; set; }

    private Vector2 _keystrokeTrack;

    private IReceiverEnhancedAsync<PlayerJumpController, bool> _jumpReceiver;

    private CommandAsyncEnhanced<PlayerJumpController, bool> _jumpCommand;

    private IReceiverEnhancedAsync<PlayerSlideController, PlayerStateBundle> _slideReceiver;

    private CommandAsyncEnhanced<PlayerSlideController, PlayerStateBundle> _slideCommand;

    private IReceiverEnhancedAsync<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>> _attackReceiver;

    private CommandAsyncEnhanced<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>> _attackCommand;

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>> _animationCommand;

    private IReceiverEnhancedAsync<PlayerLedgeGrabController, PlayerStateBundle> _ledgeGrabReceiver;

    private CommandAsyncEnhanced<PlayerLedgeGrabController, PlayerStateBundle> _ledgeGrabCommand;

    private IReceiver<bool> _throwingProjectileReceiver;

    private Command<bool> _throwingProjectileCommand;

    private PlayerActionsModel _playerActionsModel;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>()
    {
        StateBundle = new GameStateBundle()
    };

    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>()
    {
        StateBundle = new PlayerStateBundle()
    };

    private ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private GlobalGameStateDelegator _globalGameStateDelegator;

    private PlayerVelocityDelegator _playerVelocityDelegator;

    private PlayerStateDelegator _playerStateDelegator;

    private PlayerAttributesDelegator _playerAttributesDelegator;


    //Force = -2m * sqrt (g * h)
    private async void Awake()
    {
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions

        _playerActionsModel = new PlayerActionsModel();

        _jumpReceiver = await Helper.FindReceiver<PlayerJumpController, IReceiverBase<bool>>();

        _slideReceiver = await Helper.FindReceiver<PlayerSlideController, IReceiverBase<PlayerStateBundle>>();

        _ledgeGrabReceiver = await Helper.FindReceiver<PlayerLedgeGrabController, IReceiverBase<PlayerStateBundle>>();

        _attackReceiver = await Helper.FindReceiver<PlayerAttackController, IReceiverBase<ControllerPackage<AttackingExecutionState, AttackingDetails>>>();

        _throwingProjectileReceiver = await  Helper.FindReceiver<ThrowingProjectileController, IReceiverBase<bool>>();

        _animationReceiver = await Helper.FindReceiver<PlayerAnimationController, IReceiverBase<ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _attackCommand = new CommandAsyncEnhanced<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>>(_attackReceiver);

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationReceiver);

        _jumpCommand = new CommandAsyncEnhanced<PlayerJumpController, bool>(_jumpReceiver);

        _slideCommand = new CommandAsyncEnhanced<PlayerSlideController, PlayerStateBundle>(_slideReceiver);

        _ledgeGrabCommand = new CommandAsyncEnhanced<PlayerLedgeGrabController, PlayerStateBundle>(_ledgeGrabReceiver);

        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _playerActionsModel.CharacterSpeed = new Vector2(_characterSpeed, 0f);

        _playerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        _rocky2DActions.PlayerMovement.Movement.started += MovementBegin;

        _rocky2DActions.PlayerMovement.Movement.canceled += MovementCancelled;

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
        InvokeCustomMethod += NotifySubjects;

        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.BoostAttack.Enable();
    }

    private void FixedUpdate()
    {
        Player.Rigidbody.linearVelocity = _playerActionsModel.CharacterVelocity;
    }

    private async void NotifySubjects()
    {
        _playerVelocityDelegator = await Helper.GetDelegator<PlayerVelocityDelegator>();

        _playerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        _playerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        _globalGameStateDelegator = await Helper.GetDelegator<GlobalGameStateDelegator>();

        _playerVelocityDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerSlideController).ToString()
        }, CancellationToken.None);

        _playerVelocityDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerJumpController).ToString()
        }, CancellationToken.None);

        _playerStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None);

        _playerAttributesDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None);

        _globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(GameStateConsumer).ToString()
        }, CancellationToken.None);
    }


    #region Controller Mechnism
    private async void MovementBegin(InputAction.CallbackContext context)
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>();

        Debug.Log($"MovementBegin {keystroke}");

        _playerActionsModel.KeyStrokeDifference = Vector2.zero.x + keystroke.x;

        _playerActionsModel.CharacterVelocity = new Vector2(keystroke.x, _playerActionsModel.CharacterVelocity.y) * _playerActionsModel.CharacterSpeed;

        CurrentPlayerState.StateBundle.PlayerMovementState = new State<MovementState, MovementDto> { CurrentState = _playerActionsModel.KeyStrokeDifference == 0 ? MovementState.IS_IDLE : 
            MovementState.IS_RUNNING, CurrentValue =  new MovementDto() { CharacterSpeed = new Vector2(Math.Abs(Player.Rigidbody.linearVelocity.x), Math.Abs(Player.Rigidbody.linearVelocity.y)) }, IsConcluded = false };

        await _playerStateEvent.Invoke(CurrentPlayerState);

        await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>()
        {
            ExecutionState = AnimationExecutionState.MOVEMENT,
            Value =  CurrentPlayerState.StateBundle
        });

        if (KeystrokeMagnitudeChecker(keystroke))
        {
            FlipCharacter(keystroke);
        }
    }

    private async void MovementCancelled(InputAction.CallbackContext context)
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>();

        Debug.Log($"MovementCancelled {keystroke}");

        _playerActionsModel.CharacterVelocity = new Vector2(keystroke.x, _playerActionsModel.CharacterVelocity.y);
    }

    private void VelocityYEventHandler(float characterVelocityY)
    {
        _playerActionsModel.CharacterVelocity = new Vector2(_playerActionsModel.CharacterVelocity.x, characterVelocityY);

        Debug.Log($"In the event Y handler! {_playerActionsModel.CharacterVelocity}");
    }

    private bool KeystrokeMagnitudeChecker(Vector2 _keystrokeTrack)
    {
        return _keystrokeTrack.magnitude != 0;
    }

    private async void BeginJumpAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetJumpPressed = _playerActionsModel.GetSlidePressed ? false : context.ReadValueAsButton();

        await _jumpCommand.Execute(_playerActionsModel.GetJumpPressed);
    }

    private async void EndJumpAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetJumpPressed = _playerActionsModel.GetSlidePressed ? false : context.ReadValueAsButton();

        await _jumpCommand.Execute(_playerActionsModel.GetJumpPressed);
    }

    private async void BeginSlideAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState == AttackState.IS_ATTACKING) ? false : context.ReadValueAsButton();

        CurrentPlayerState.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>() { CurrentState = MovementState.IS_SLIDING, CurrentValue = new MovementDto() { CharacterSpeed = _playerActionsModel.CharacterSpeed,  }, IsConcluded = true };

        await _playerStateEvent.Invoke(CurrentPlayerState);

        await _slideCommand.Execute();
    }
    private async void EndSlideAction(InputAction.CallbackContext context)
    {
        _playerActionsModel.GetSlidePressed = (_playerActionsModel.GetJumpPressed == true || CurrentPlayerState.StateBundle.PlayerAttackState.CurrentState == AttackState.IS_ATTACKING) ? false : context.ReadValueAsButton();

        await _slideCommand.Cancel();
    }

    //attacking mechanism centralized
    private void HandleDaggerInput(InputAction.CallbackContext context)
    {
        _playerActionsModel.DaggerInput = context.ReadValueAsButton();

        ThrowingProjectileController.InvokeThrowableProjectileEvent(_playerActionsModel.DaggerInput);

        _throwingProjectileCommand.Execute();
    }

    private async void HandlePlayerAttackCancel(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickEnd = (float)context.time;

        await _attackCommand.Execute(new ControllerPackage<AttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = AttackingExecutionState.ON_CLICK_EVENT,
            Value = new AttackingDetails()
            {
                MouseClickDto = new MouseClickDto()
                {
                    ClickStartTime  = _playerActionsModel.TimeForMouseClickStart,
                    ClickEndTime = _playerActionsModel.TimeForMouseClickEnd
                }
            }
        });

        await _attackCommand.Cancel(new ControllerPackage<AttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = AttackingExecutionState.ATTACKING_ACTION,
            Value = new AttackingDetails()
            { 
                AttackingValue = _playerActionsModel.LeftMouseButtonPressed
            } 
        });
    }

    private async void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickStart = (float)context.time;

        //send time stamps to the attacking controller
        await _attackCommand.Execute(new ControllerPackage<AttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = AttackingExecutionState.ON_CLICK_EVENT,
            Value = new AttackingDetails()
            {
                MouseClickDto = new MouseClickDto()
                {
                    ClickStartTime = _playerActionsModel.TimeForMouseClickStart,
                    ClickEndTime = _playerActionsModel.TimeForMouseClickEnd
                }
            }
        });

        //execute Attack
        await _attackCommand.Execute(new ControllerPackage<AttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = AttackingExecutionState.ATTACKING_ACTION,
            Value = new AttackingDetails()
            {
                AttackingValue = _playerActionsModel.LeftMouseButtonPressed
            }
        });
    }

    //boost v attack
    private async void HandleBoostAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.VBoostKeyPressed = context.ReadValueAsButton();

        await _attackCommand.Execute(new ControllerPackage<AttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = AttackingExecutionState.BOOST_ATTACK,
            Value = new AttackingDetails()
            {
                AttackingValue = _playerActionsModel.VBoostKeyPressed
            }
        });
    }
    private void HandleBoostAttackCancel(InputAction.CallbackContext context)
    {
        _playerActionsModel.VBoostKeyPressed = context.ReadValueAsButton();
    }


    #region Observer Pattern

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
        if (_playerActionsModel.KeyStrokeDifference == -1 && Player.Transform.localScale.x < 0)
        {
            Player.Transform.localScale = new Vector3(1 * Player.Transform.localScale.x, Player.Transform.localScale.y, Player.Transform.localScale.z);

        }
        else if (_playerActionsModel.KeyStrokeDifference == 1 && Player.Transform.localScale.x < 0 || _playerActionsModel.KeyStrokeDifference == -1 && Player.Transform.localScale.x > 0)
        {
            Player.Transform.localScale = new Vector3(-1 * Player.Transform.localScale.x, Player.Transform.localScale.y, Player.Transform.localScale.z);
        }
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }

    #endregion
}
