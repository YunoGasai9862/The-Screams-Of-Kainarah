using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>
{
    private const float FALLING_SPEED = 0.8f;
    private const float JUMPING_SPEED = 0.5f;
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private Rigidbody2D _rb;
    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private CapsuleCollider2D _capsuleCollider;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxJumpHeight;

    private float _characterVelocityY;
    private float _timeCounter;
    private bool _isJumpPressed;
    private Vector3 _playerInitialPosition;

    public PlayerJumpEvent onPlayerJumpEvent;
    public float CharacterVelocityY { get => _characterVelocityY; set => _characterVelocityY = value; }
    private Vector3 PlayerInitialPosition { get => _playerInitialPosition; set=> _playerInitialPosition = value; }  

    public bool CancelAction()
    {
        PlayerVariables.Instance.jumpVariableEvent.Invoke(false);
        return true;
    }
    public bool PerformAction(bool value)
    {
        SetPlayerInitialPosition(PlayerVariables.Instance.IS_JUMPING);
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
         //_animationHandler.UpdateJumpTime(AnimationConstants.JUMP_TIME, JumpTime);
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

            _isJumpPressed = false;

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

        if (await CanPlayerFall(maxJumpHeight)) //peak reached
        {
            PlayerVariables.Instance.jumpVariableEvent.Invoke(false);
        }
    }

    private Task<bool> CanPlayerJump()
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(!isJumping, isOnLedgeOrGround, isJumpPressed));
    }

    private Task SetPlayerInitialPosition(bool isJumping)
    {
        if(LedgeGroundChecker(groundLayer, ledgeLayer) && !isJumping)
        {
            PlayerInitialPosition = transform.position;
        }
        return Task.CompletedTask;
    }
    private async Task<bool> CanPlayerFall(float maxJumpHeight)
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool isJumpPressed = _isJumpPressed;
        return MovementHelperFunctions.boolConditionAndTester(isJumping, !isOnLedgeOrGround, isJumpPressed, await MaxJumpHeightChecker(maxJumpHeight));
    }
    private bool LedgeGroundChecker(LayerMask ground, LayerMask ledge)
    {
        return _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, ground, COLLIDER_DISTANCE_FROM_THE_LAYER)
            || _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }

    public async Task<bool> MaxJumpHeightChecker(float maxJumpHeight)
    {
        if(transform.position.y >= PlayerInitialPosition.y + maxJumpHeight )
        {
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

}