using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>
{
    private const float FALLING_SPEED = 0.8f;
    private const float JUMPING_SPEED = 0.5f;
    private const float MAX_FALL_TIME = 0.5f;

    private Rigidbody2D _rb;
    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private CapsuleCollider2D _capsuleCollider;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxTimeJump;

    private float _characterVelocityY;
    private float _timeCounter;
    private bool _isJumpPressed;

    public PlayerJumpEvent onPlayerJumpEvent;
    public float CharacterVelocityY { get => _characterVelocityY; set => _characterVelocityY = value; }
    private float JumpTime { get => _timeCounter; } 
    public bool CancelAction()
    {
        PlayerVariables.Instance.jumpVariableEvent.Invoke(false);
        return true;
    }

    public bool PerformAction(bool value)
    {
        _isJumpPressed = true;
        return true;
    }
    private void Awake()
    {
        _movementHelperClass = new MovementHelperClass();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _rb = GetComponent<Rigidbody2D>();
        onPlayerJumpEvent = new PlayerJumpEvent();
    }
    private async void Update()
    {
        await HandleJumpingMechanism();
    }
    private void FixedUpdate()
    {
        if (_rb.velocity.y > 0f) //how high the player can jump
        {
            _timeCounter += Time.fixedDeltaTime;
        }
      //  _animationHandler.UpdateJumpTime(AnimationConstants.JUMP_TIME, JumpTime);
    }
    public async Task HandleJumpingMechanism()
    {
        await HandleJumping();

        await HandleFalling();

        onPlayerJumpEvent.Invoke(CharacterVelocityY);

        await Task.FromResult(true);
    }

    public async Task HandleFalling()
    {
        if (!PlayerVariables.Instance.IS_JUMPING && !LedgeGroundChecker(groundLayer, ledgeLayer)) //falling
        {
            CharacterVelocityY = -JumpSpeed * FALLING_SPEED; //extra check

            _animationHandler.JumpingFallingAnimationHandler(false);
        }

        if (LedgeGroundChecker(groundLayer, ledgeLayer) && !_isJumpPressed) //on the ground
        {
            CharacterVelocityY = 0f;

            _timeCounter = 0;
        }

        await Task.FromResult(true);
    }

    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) //jumping
        {
            PlayerVariables.Instance.jumpVariableEvent.Invoke(true);

            CharacterVelocityY = JumpSpeed * JUMPING_SPEED;

            _animationHandler.JumpingFallingAnimationHandler(true); //jumping animation
        }

        if (await CanPlayerFall() || await MaxJumpTimeChecker()) //peak reached
        {
            PlayerVariables.Instance.jumpVariableEvent.Invoke(false);

            _isJumpPressed = false; //fixed the issue of eternally looping at jump on JUMP HOLD

        }
    }

    private Task<bool> CanPlayerJump()
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(!isJumping, isOnLedgeOrGround, isJumpPressed));
    }

    private Task<bool> CanPlayerFall()
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(isJumping, !isOnLedgeOrGround, !isJumpPressed));
    }
    private bool LedgeGroundChecker(LayerMask ground, LayerMask ledge)
    {
        return _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, ground)
            || _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, ledge);
    }
    public Task<bool> MaxJumpTimeChecker()
    {
        return Task.FromResult(_timeCounter > maxTimeJump);
    }

}