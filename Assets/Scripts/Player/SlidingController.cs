using CoreCode;
using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IReceiverAsync<bool>, ISubject<IObserver<CharacterVelocity>>
{
    private const float MAX_ANIMATION_TIME = 0.6f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float slidingSpeed;

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private PlayerAnimationMethods _animationHandler;

    private IOverlapChecker _movementHelperClass;

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Collider2D _col;

    private Animator _anim;

    private Rigidbody2D _rb;

    private bool IS_SLIDING { get; set; } = false;

    private void Awake()
    {
        PlayerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();
    }
    void Start()
    {
        PlayerVelocityDelegator.AddToSubjectsDict(typeof(SlidingController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());
        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(SlidingController).ToString())[name].SetSubject(this);

        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = new MovementHelperClass();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _col= GetComponent<CapsuleCollider2D>();
    }

    private Task Slide()
    {
        if (IS_SLIDING && _movementHelperClass.OverlapAgainstLayerMaskChecker(ref _col, groundLayer, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            onSlideEvent.Invoke(slidingSpeed); //posting for speed

            _animationHandler.Sliding(true); //set animation
        }

        if (_animationHandler.ReturnCurrentAnimation() > MAX_ANIMATION_TIME && _animationHandler.IsNameOfTheCurrentAnimation(PlayerAnimationConstants.SLIDING))
        {
            PlayerSystem.slideVariableEvent.PlayerSlideStateEventInvoke(true);

            _animationHandler.Sliding(false);
        }

        return Task.CompletedTask;
    }

    async Task<bool> IReceiverAsync<bool>.PerformAction(bool value)
    {
        if (await IsVelocityXGreaterThanZero(_rb) && !_playerAttackStateMachine.IsInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())
        {
            IS_SLIDING = true;

            FlagDelegator.NotifyObservers(IS_SLIDING, gameObject.name, typeof(SlidingController), CancellationToken.None);

            await Slide();
        }
        return await Task.FromResult(true);
    }
    async Task<bool> IReceiverAsync<bool>.CancelAction()
    {
        IS_SLIDING = false;

        PlayerVelocityDelegator.NotifyObservers(IS_SLIDING, gameObject.name, typeof(SlidingController), CancellationToken.None);

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

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(SlidingController).ToString())[gameObject.name], observer);
    }
}
