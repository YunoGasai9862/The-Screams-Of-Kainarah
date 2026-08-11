using Assets.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerActions), SubjectType = typeof(PlayerStateConsumer), ContextType = typeof(GenericStateBundle<PlayerStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerActions), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerActions), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerActions), SubjectType = typeof(PlayerSlideController), ContextType = typeof(CharacterVelocity))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerActions), SubjectType = typeof(PlayerJumpController), ContextType = typeof(CharacterVelocity))]
public class PlayerActions : MonoBehaviorScene, INotify<GenericStateBundle<PlayerStateBundle>>, INotify<Player>, INotify<GenericStateBundle<GameStateBundle>>, INotify<CharacterVelocity>
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

    private SceneUtils SceneUtils {  get; set; }

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>()
    {
        StateBundle = new GameStateBundle()
    };

    private GenericStateBundle<PlayerStateBundle> CurrentPlayerState { get; set; } = new GenericStateBundle<PlayerStateBundle>()
    {
        StateBundle = new PlayerStateBundle()
    };

    private ThrowingProjectileController ThrowingProjectileController { get => GetComponent<ThrowingProjectileController>(); } //implement all the actions together

    private Delegator Delegator { get; set; }

    //Force = -2m * sqrt (g * h)
    private async void Start()
    {
        SceneUtils = await(await GetBaseScene()).GetSceneUtilsAsync();

        Delegator = SceneUtils.GetDelegator();

        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions

        _playerActionsModel = new PlayerActionsModel();

        _jumpReceiver = await SceneUtils.FindReceiver<PlayerJumpController, IReceiverBase<bool>>();

        _slideReceiver = await SceneUtils.FindReceiver<PlayerSlideController, IReceiverBase<PlayerStateBundle>>();

        _ledgeGrabReceiver = await SceneUtils.FindReceiver<PlayerLedgeGrabController, IReceiverBase<PlayerStateBundle>>();

        _attackReceiver = await SceneUtils.FindReceiver<PlayerAttackController, IReceiverBase<ControllerPackage<AttackingExecutionState, AttackingDetails>>>();

        _throwingProjectileReceiver = await SceneUtils.FindReceiver<ThrowingProjectileController, IReceiverBase<bool>>();

        _animationReceiver = await SceneUtils.FindReceiver<PlayerAnimationController, IReceiverBase<ControllerPackage<AnimationExecutionState, PlayerStateBundle>>>();

        _attackCommand = new CommandAsyncEnhanced<PlayerAttackController, ControllerPackage<AttackingExecutionState, AttackingDetails>>(_attackReceiver);

        _animationCommand = new CommandAsyncEnhanced<PlayerAnimationController, ControllerPackage<AnimationExecutionState, PlayerStateBundle>>(_animationReceiver);

        _jumpCommand = new CommandAsyncEnhanced<PlayerJumpController, bool>(_jumpReceiver);

        _slideCommand = new CommandAsyncEnhanced<PlayerSlideController, PlayerStateBundle>(_slideReceiver);

        _ledgeGrabCommand = new CommandAsyncEnhanced<PlayerLedgeGrabController, PlayerStateBundle>(_ledgeGrabReceiver);

        _throwingProjectileCommand = new Command<bool>(_throwingProjectileReceiver);

        _playerActionsModel.CharacterSpeed = new Vector2(_characterSpeed, 0f);

        _playerStateEvent = await SceneUtils.GetCustomEvent<PlayerStateEvent>();

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

        Debug.Log($"Rocky 2D Actions: {_rocky2DActions}, {_rocky2DActions.PlayerMovement}");

        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

        _rocky2DActions.PlayerAttack.Attack.Enable(); //activates the Action Map

        _rocky2DActions.PlayerAttack.ThrowProjectile.Enable();

        _rocky2DActions.PlayerAttack.BoostAttack.Enable();
    }

    private void FixedUpdate()
    {
        if (Player == null)
        {
            Debug.Log($"Player Rigidbody reference is missing");

            return;
        }

        Player.Rigidbody.linearVelocity = _playerActionsModel.CharacterVelocity;
    }

    #region Controller Mechnism
    private async void MovementBegin(InputAction.CallbackContext context)
    {
        Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>();

        _playerActionsModel.KeyStrokeDifference = Vector2.zero.x + keystroke.x;

        _playerActionsModel.CharacterVelocity = new Vector2(keystroke.x, _playerActionsModel.CharacterVelocity.y) * _playerActionsModel.CharacterSpeed;

        CurrentPlayerState.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>
        {
            CurrentState = _playerActionsModel.KeyStrokeDifference == 0 ? MovementState.IS_IDLE : MovementState.IS_RUNNING,
            CurrentValue = new MovementDto()
            {
                CharacterSpeed = _playerActionsModel.KeyStrokeDifference == 0 ? Vector2.zero : new Vector2(Math.Abs(_playerActionsModel.CharacterVelocity.x), Math.Abs(_playerActionsModel.CharacterVelocity.x))
            },
            IsConcluded = false
        };


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

        _playerActionsModel.CharacterVelocity = new Vector2(keystroke.x, _playerActionsModel.CharacterVelocity.y);

        CurrentPlayerState.StateBundle.PlayerMovementState = new State<MovementState, MovementDto>
        {
            CurrentState =  MovementState.IS_IDLE,
            CurrentValue = new MovementDto()
            {
                CharacterSpeed = Vector2.zero
            },
            IsConcluded = false
        };

        await _playerStateEvent.Invoke(CurrentPlayerState);

        await _animationCommand.Execute(new ControllerPackage<AnimationExecutionState, PlayerStateBundle>()
        {
            ExecutionState = AnimationExecutionState.MOVEMENT,
            Value =  CurrentPlayerState.StateBundle
        });
    }

    private void VelocityYEventHandler(float characterVelocityY)
    {
        _playerActionsModel.CharacterVelocity = new Vector2(_playerActionsModel.CharacterVelocity.x, characterVelocityY);
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

        await _jumpCommand.Cancel(_playerActionsModel.GetJumpPressed);
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

    public IEnumerator Notify(GenericStateBundle<PlayerStateBundle> value)
    {
        CurrentPlayerState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        CurrentGameState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator Notify(CharacterVelocity value)
    {
        VelocityYEventHandler(value.VelocityY);

        yield return null;
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


    #endregion
}
