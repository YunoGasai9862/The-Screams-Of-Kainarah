using PlayerAnimationHandler;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>
{
    private const float FALLING_SPEED = 0.8f;
    private const float JUMPING_SPEED = 0.5f;
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    private const float MAX_JUMP_TIME = 0.3f;

    private Rigidbody2D _rb;
    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private Collider2D _col;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxJumpHeight;

    private float _characterVelocityY;
    private bool _isJumpPressed;
    private Vector3 _playerInitialPosition;

    public PlayerJumpVelocityEvent onPlayerJumpEvent;
    public PlayerJumpTimeEvent onPlayerJumpTimeEvent;
    public float CharacterVelocityY { get => _characterVelocityY; set => _characterVelocityY = value; }
    public Vector3 PlayerInitialPosition { get => _playerInitialPosition; set=> _playerInitialPosition = value; }
    public float TimeEclipsed { get; set; }

    public bool CancelAction()
    {
        PlayerVariables.Instance.jumpVariableEvent.Invoke(false);
        return true;
    }
    public bool PerformAction(bool value)
    {
        _isJumpPressed = value;
        SetPlayerInitialPosition(PlayerVariables.Instance.IS_JUMPING);
        return true;
    }
    private void Awake()
    {
        _movementHelperClass = new MovementHelperClass();
        _col = GetComponent<CapsuleCollider2D>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        onPlayerJumpEvent = new PlayerJumpVelocityEvent();
        onPlayerJumpTimeEvent = new PlayerJumpTimeEvent(MAX_JUMP_TIME);
        onPlayerJumpTimeEvent.AddListener(MaxTimePassed);
        onPlayerJumpEvent.Invoke(CharacterVelocityY = -10f);
    }
    private async void Update()
    {
        await HandleJumpingMechanism();

        if (PlayerVariables.Instance.IS_JUMPING && !PlayerVariables.Instance.IS_GRABBING)
        {
            TimeEclipsed += Time.deltaTime;
        }

        onPlayerJumpTimeEvent.ShouldFall(TimeEclipsed);
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
        if (await CanPlayerFall(maxJumpHeight) || !_isJumpPressed || onPlayerJumpTimeEvent.Fall) //falling
        {
            CharacterVelocityY = -JumpSpeed * FALLING_SPEED;
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && await IsYVelocityNegative(_rb))
        {
            _animationHandler.JumpingFallingAnimationHandler(false);
        }

        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed) //on the ground
        {
            PlayerVariables.Instance.jumpVariableEvent.Invoke(false);

            onPlayerJumpTimeEvent.Fall = false;

            TimeEclipsed = 0;
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

    }

    private Task<bool> CanPlayerJump()
    {
        bool isJumping = PlayerVariables.Instance.IS_JUMPING;
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer));
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(!isJumping, isOnLedgeOrGround, isJumpPressed));
    }

    private Task SetPlayerInitialPosition(bool isJumping)
    {
        if((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !isJumping)
        {
            PlayerInitialPosition = transform.position;
        }
        return Task.CompletedTask;
    }
    private async Task<bool> CanPlayerFall(float maxJumpHeight)
    {
        bool isOnLedgeOrGround = (IsOnTheGround(groundLayer) && IsOnTheLedge(ledgeLayer));
        return MovementHelperFunctions.boolConditionAndTester(!isOnLedgeOrGround, await MaxJumpHeightChecker(maxJumpHeight));
    }
    private bool IsOnTheGround(LayerMask ground)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, ground, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    private bool IsOnTheLedge(LayerMask ledge)
    {
        return _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER);
    }
    public async Task<bool> MaxJumpHeightChecker(float maxJumpHeight)
    {
        if(transform.position.y >= PlayerInitialPosition.y + maxJumpHeight )
        {
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public void MaxTimePassed(bool value)
    {
        onPlayerJumpTimeEvent.Fall = value;
    }

    private Task<bool> IsYVelocityNegative(Rigidbody2D rb)
    {
        return rb.linearVelocity.y < 0 ? Task.FromResult(true) : Task.FromResult(false);
    }

}