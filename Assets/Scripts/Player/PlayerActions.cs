using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngineInternal;

public class PlayerActions : MonoBehaviour, IObserver<GenericStateBundle<PlayerStateBundle>>, IObserver<GenericStateBundle<GameStateBundle>>, IObserver<CharacterSpeed>, IObserver<CharacterVelocity>, IObserver<IEntityRigidBody>
{
    [SerializeField] float _characterSpeed = 10f;

    private PlayerStateEvent _playerStateEvent;

    private Rocky2DActions _rocky2DActions;

    private Vector2 _keystrokeTrack;

    private IReceiverEnhancedAsync<JumpingController, bool> _jumpReceiver;

    private CommandAsyncEnhanced<JumpingController, bool> _jumpCommand;

    private IReceiverEnhancedAsync<SlidingController, bool> _slidingReceiver;

    private CommandAsyncEnhanced<SlidingController, bool> _slidingCommand;

    private IReceiverAsync<bool> _slideReceiver;

    private CommandAsync<bool> _slideCommand;

    private IReceiverEnhancedAsync<AttackingController, ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>> _attackReceiver;

    private CommandAsyncEnhanced<AttackingController, ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>> _attackCommand;

    private IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationReceiver;

    private CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>> _animationCommand;

    private IReceiver<bool> _throwingProjectileReceiver;

    private Command<bool> _throwingProjectileCommand;

    private PlayerActionsModel _playerActionsModel;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together
   
    private GlobalGameStateDelegator _globalGameStateDelegator;

    private PlayerVelocityDelegator _playerVelocityDelegator;

    private PlayerStateDelegator _playerStateDelegator;

    private PlayerAttributesDelegator _playerAttributesDelegator;

    private Rigidbody2D _rb;


    //Force = -2m * sqrt (g * h)
    private void Awake()
    {

        _globalGameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        _playerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();

        _playerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        _playerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();

        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions

        _playerActionsModel = new PlayerActionsModel();

        _jumpReceiver = Helper.FindReceiver<JumpingController, IReceiverBase<bool>>();

        _slideReceiver = Helper.FindReceiver<SlidingController, IReceiverBase<bool>>();

        _attackReceiver = Helper.FindReceiver<AttackingController, IReceiverBase<ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>>();

        _throwingProjectileReceiver = Helper.FindReceiver<ThrowingProjectileController, IReceiverBase<bool>>();

        _animationReceiver = Helper.FindReceiver<PlayerAnimationController, IReceiverBase<ControllerPackage<PlayerAnimationExecutionState, bool>>>();

        _attackCommand = new CommandAsyncEnhanced<AttackingController, ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>>(_attackReceiver);

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>(_animationReceiver);

        _jumpCommand = new CommandAsyncEnhanced<JumpingController, bool>(_jumpReceiver);

        _slideCommand = new CommandAsync<bool>(_slideReceiver);

        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _playerActionsModel.OriginalSpeed = _characterSpeed;

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
        _globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GameStateConsumer).ToString()

        }, CancellationToken.None);

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

        StartCoroutine(_playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));

        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.BoostAttack.Enable();
    }

    private async void Update()
    {
        if (CurrentGameState == null || CurrentGameState.StateBundle == null)
        {
            Debug.Log("CurrentGameState || CurrentGameState.StateBundle is null - skipping update (PlayerActions)!");
            return;
        }

        if (CurrentPlayerState == null || CurrentPlayerState.StateBundle == null)
        {
            Debug.Log("CurrentPlayerState || CurrentPlayerState.StateBundle is null - skipping update (PlayerActions)!");
            return;
        }


        if (CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE)) 
        {
            await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>() { ExecutionState = PlayerAnimationExecutionState.PLAY_MOVEMENT_ANIMATION, Value = false });
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

         await _animationCommand.Execute(new ControllerPackage<PlayerAnimationExecutionState, bool>()
        {
            ExecutionState = PlayerAnimationExecutionState.PLAY_MOVEMENT_ANIMATION,
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

    private async void HandlePlayerAttackCancel(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = IsSlidingActionInProgress(CurrentPlayerState.StateBundle) ? false : context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickEnd = (float)context.time;

        await _attackCommand.Execute(new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = PlayerAttackingExecutionState.ON_CLICK_EVENT,
            Value = new AttackingDetails()
            {
                AttackingStartTime = _playerActionsModel.TimeForMouseClickStart,
                AttackingEndTime = _playerActionsModel.TimeForMouseClickEnd
            }
        });

        await _attackCommand.Cancel(new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = PlayerAttackingExecutionState.ATTACKING_ACTION,
            Value = new AttackingDetails()
            { 
                AttackingValue = _playerActionsModel.LeftMouseButtonPressed
            } 
        });
    }

    private async void HandlePlayerAttackStart(InputAction.CallbackContext context)
    {
        _playerActionsModel.LeftMouseButtonPressed = IsSlidingActionInProgress(CurrentPlayerState.StateBundle) ? false : context.ReadValueAsButton();
        _playerActionsModel.TimeForMouseClickStart = (float)context.time;

        //send time stamps to the attacking controller
        await _attackCommand.Execute(new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = PlayerAttackingExecutionState.ON_CLICK_EVENT,
            Value = new AttackingDetails()
            {
                AttackingStartTime = _playerActionsModel.TimeForMouseClickStart,
                AttackingEndTime = _playerActionsModel.TimeForMouseClickEnd
            }
        });

        //execute Attack
        await _attackCommand.Execute(new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = PlayerAttackingExecutionState.ATTACKING_ACTION,
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

        await _attackCommand.Execute(new ControllerPackage<PlayerAttackingExecutionState, AttackingDetails>()
        {
            ExecutionState = PlayerAttackingExecutionState.BOOST_ATTACK,
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
        Debug.Log($"PlayerStateBundle in PlayerActions - {data.StateBundle}");

        CurrentPlayerState.StateBundle = data.StateBundle;
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Debug.Log($"GameStateBundle in PlayerActions - {data.StateBundle}");

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

    public void OnNotify(IEntityRigidBody data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        _rb = data.Rigidbody;
    }

    #endregion
}
