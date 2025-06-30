using CoreCode;
using PlayerAnimationHandler;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IObserver<PlayerSystem>, IReceiverAsync<bool>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float slidingSpeed;

    [SerializeField] PlayerSystemDelegator playerSystemDelegator;

    private PlayerAnimationMethods _animationHandler;

    private IOverlapChecker _movementHelperClass;

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Collider2D _col;

    private Animator _anim;

    private Rigidbody2D _rb;

    private PlayerSystem PlayerSystem { get; set; }

    public OnSlidingEvent onSlideEvent = new OnSlidingEvent();


    void Start()
    {
        StartCoroutine(playerSystemDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerSystem).ToString()
        }, CancellationToken.None));


        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = new MovementHelperClass();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _col= GetComponent<CapsuleCollider2D>();
    }

    private Task Slide()
    {
        if (PlayerSystem.IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            onSlideEvent.Invoke(slidingSpeed); //posting for speed

            _animationHandler.Sliding(true); //set animation
        }

        if (_animationHandler.ReturnCurrentAnimation() > MAX_ANIMATION_TIME && _animationHandler.IsNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
        {
            PlayerSystem.slideVariableEvent.Invoke(false);

            PlayerSystem.slideVariableEvent.PlayerSlideStateEventInvoke(true);

            _animationHandler.Sliding(false);

        }

        return Task.CompletedTask;
    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        if (await IsVelocityXGreaterThanZero(_rb) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            PlayerSystem.slideVariableEvent.Invoke(true);

            await Slide();
        }
        return await Task.FromResult(true);
    }
    async Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        PlayerSystem.slideVariableEvent.Invoke(false);

        return await Task.FromResult(true);
    }

    private Task<bool> IsVelocityXGreaterThanZero(Rigidbody2D rb)
    {
        return Task.FromResult(Mathf.Abs(rb.linearVelocity.x) > 0);
    }

    public void OnNotify(PlayerSystem data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerSystem = data;
    }
}
