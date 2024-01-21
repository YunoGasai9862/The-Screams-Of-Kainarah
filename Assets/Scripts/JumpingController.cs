using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>
{
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
    public bool CancelAction()
    {
        return false;
    }

    public bool PerformAction(bool value)
    {
        _isJumpPressed = value;
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
        await HandleJumping();
    }
    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) //jumping
        {
            PlayerVariables.jumpVariableEvent.Invoke(true);

            CharacterVelocityY = JumpSpeed * .5f;

            _animationHandler.JumpingFalling(PlayerVariables.IS_JUMPING); //jumping animation
        }

        if (await CanPlayerFall() || await MaxJumpTimeChecker()) //peak reached
        {
            PlayerVariables.jumpVariableEvent.Invoke(false);
            CharacterVelocityY = -JumpSpeed * .8f;

            _isJumpPressed = false; //fixed the issue of eternally looping at jumep on JUMP HOLD

        }

        if (!PlayerVariables.IS_JUMPING && !LedgeGroundChecker(groundLayer, ledgeLayer)) //falling
        {
            CharacterVelocityY = -JumpSpeed * .8f; //extra check
            _animationHandler.JumpingFalling(PlayerVariables.IS_JUMPING); //falling naimation

        }

        if (_rb.velocity.y > 0f) //how high the player can jump
        {
            _timeCounter += Time.deltaTime;
        }

        if (!PlayerVariables.IS_JUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && !_isJumpPressed) //on the ground
        {
            CharacterVelocityY = 0f;
            _timeCounter = 0;
        }

        onPlayerJumpEvent.Invoke(CharacterVelocityY);
        await Task.FromResult(true);
    }

    private Task<bool> CanPlayerJump()
    {
        bool isJumping = PlayerVariables.IS_JUMPING;
        bool isOnLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool isJumpPressed = _isJumpPressed;

        return Task.FromResult(MovementHelperFunctions.boolConditionAndTester(!isJumping, isOnLedgeOrGround, isJumpPressed));
    }

    private Task<bool> CanPlayerFall()
    {
        bool isJumping = PlayerVariables.IS_JUMPING;
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
