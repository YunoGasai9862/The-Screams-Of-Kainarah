using CoreCode;
using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    [SerializeField] LayerMask groundLayer;
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
        if (PlayerVariables.Instance.IS_SLIDING && _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, groundLayer))
        {
            onSlideEvent.Invoke(slidingSpeed); //posting for speed

            _animationHandler.Sliding(true); //set animation
        }

        if (_animationHandler.returnCurrentAnimation() > MAX_ANIMATION_TIME && _animationHandler.isNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
        {
            _animationHandler.Sliding(false);
        }

        return Task.CompletedTask;
    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        PlayerVariables.Instance.slideVariableEvent.Invoke(true);

        if (!_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            await Slide();
        }
        return await Task.FromResult(true);
    }
    async Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        PlayerVariables.Instance.slideVariableEvent.Invoke(false);

        return await Task.FromResult(true);
    }
}
