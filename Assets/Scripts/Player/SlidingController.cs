using CoreCode;
using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>
{
    private const float MAX_ANIMATION_TIME = 0.6f;
    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float slidingSpeed;

    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private CapsuleCollider2D _capsuleCollider;
    private Animator _anim;
    private Rigidbody2D _rb;

    public OnSlidingEvent onSlideEvent=new OnSlidingEvent();


    void Start()
    {
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = new MovementHelperClass();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _capsuleCollider= GetComponent<CapsuleCollider2D>();
    }

    private Task Slide()
    {
        if (PlayerVariables.Instance.IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _capsuleCollider, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            onSlideEvent.Invoke(slidingSpeed); //posting for speed

            _animationHandler.Sliding(true); //set animation
        }

        if (_animationHandler.ReturnCurrentAnimation() > MAX_ANIMATION_TIME && _animationHandler.IsNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
        {
            PlayerVariables.Instance.slideVariableEvent.Invoke(false);

            _animationHandler.Sliding(false);
        }

        return Task.CompletedTask;
    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        if (await IsVelocityXGreaterThanZero(_rb) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            PlayerVariables.Instance.slideVariableEvent.Invoke(true);

            await Slide();
        }
        return await Task.FromResult(true);
    }
    async Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        PlayerVariables.Instance.slideVariableEvent.Invoke(false);

        return await Task.FromResult(true);
    }

    private Task<bool> IsVelocityXGreaterThanZero(Rigidbody2D rb)
    {
        return Task.FromResult(Mathf.Abs(rb.velocity.x) > 0);
    }
}
