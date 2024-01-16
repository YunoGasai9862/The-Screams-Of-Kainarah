using CoreCode;
using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float slidingSpeed;

    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private CapsuleCollider2D _capsuleCollider;
    private Animator _anim;

    public OnSlidingEvent onSlideEvent=new OnSlidingEvent();
    void Start()
    {
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = new MovementHelperClass();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _capsuleCollider= GetComponent<CapsuleCollider2D>();
    }

    private Task Slide()
    {
        if (PlayerMovementGlobalVariables.ISSLIDING && !PlayerMovementGlobalVariables.ISATTACKING &&
        _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, groundLayer))
        {
            _playerAttackStateMachine.ForceDisableAttacking(1);
            onSlideEvent.Invoke(slidingSpeed); //posting
        }

        if (_animationHandler.returnCurrentAnimation() > MAX_ANIMATION_TIME && _animationHandler.isNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
        {
            PlayerMovementGlobalVariables.ISSLIDING = false;
        }

        return Task.CompletedTask;
    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        PlayerMovementGlobalVariables.ISSLIDING = value;

        if (!_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            _animationHandler.Sliding(PlayerMovementGlobalVariables.ISSLIDING); //set animation

            await Slide();
        }

        return await Task.FromResult(true);
    }

    Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        return Task.FromResult(false);
    }
}
