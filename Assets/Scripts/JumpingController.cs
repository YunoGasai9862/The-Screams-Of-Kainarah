using GlobalAccessAndGameHelper;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>
{
    private Rigidbody2D _rb;
    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private CapsuleCollider2D _capsulecollider;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float JumpSpeed;
    [SerializeField] float maxTimeJump;
    [SerializeField] float slidingSpeed;

    private float characterVelocityY;
    private float characterVelocityX;
    private bool isJumpPressed;
    private float _timeCounter;
    public bool CancelAction()
    {
        throw new System.NotImplementedException();
    }

    public bool PerformAction(bool value)
    {
        _= HandleJumping();
        return true;
    }

    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) //jumping
        {
            PlayerMovementGlobalVariables.ISJUMPING = true;

            characterVelocityY = JumpSpeed * .5f;

            _animationHandler.JumpingFalling(PlayerMovementGlobalVariables.ISJUMPING); //jumping animation
        }

        if (await CanPlayerFall() || await MaxJumpTimeChecker()) //peak reached
        {
            PlayerMovementGlobalVariables.ISJUMPING = false;

            characterVelocityY = -JumpSpeed * .8f;

            isJumpPressed = false; //fixed the issue of eternally looping at jumep on JUMP HOLD

        }

        if (!PlayerMovementGlobalVariables.ISJUMPING && !LedgeGroundChecker(groundLayer, ledgeLayer)) //falling
        {
            characterVelocityY = -JumpSpeed * .8f; //extra check
            _animationHandler.JumpingFalling(PlayerMovementGlobalVariables.ISJUMPING); //falling naimation

        }

        if (_rb.velocity.y > 0f) //how high the player can jump
        {
            _timeCounter += Time.deltaTime;
        }

        if (!PlayerMovementGlobalVariables.ISJUMPING && LedgeGroundChecker(groundLayer, ledgeLayer) && !isJumpPressed) //on the ground
        {
            characterVelocityY = 0f;
            _timeCounter = 0;
        }

        await Task.FromResult(true);
    }

    private Task<bool> CanPlayerJump()
    {
        bool _isJumping = PlayerMovementGlobalVariables.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return Task.FromResult(PlayerMovementHelperFunctions.boolConditionAndTester(!_isJumping, _isOntheLedgeOrGround, _isJumpPressed));
    }

    private Task<bool> CanPlayerFall()
    {
        bool _isJumping = PlayerMovementGlobalVariables.ISJUMPING;
        bool _isOntheLedgeOrGround = LedgeGroundChecker(groundLayer, ledgeLayer);
        bool _isJumpPressed = isJumpPressed;

        return Task.FromResult(PlayerMovementHelperFunctions.boolConditionAndTester(_isJumping, !_isOntheLedgeOrGround, !_isJumpPressed));
    }
    private bool LedgeGroundChecker(LayerMask ground, LayerMask ledge)
    {
        return _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, ground)
            || _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsulecollider, ledge);
    }
    public Task<bool> MaxJumpTimeChecker()
    {
        return Task.FromResult(_timeCounter > maxTimeJump);
    }


}
