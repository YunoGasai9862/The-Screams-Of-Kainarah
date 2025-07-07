using PlayerAnimationHandler;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class JumpingController : MonoBehaviour, IReceiver<bool>, ISubject<IObserver<bool>>, ISubject<IObserver<CharacterVelocity>>
{
    [SerializeField] LayerMask groundLayer;

    [SerializeField] LayerMask ledgeLayer;

    [SerializeField] float JumpSpeed;

    [SerializeField] float maxJumpHeight;

    private const float FALLING_SPEED = 0.8f;

    private const float JUMPING_SPEED = 0.5f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private const float MAX_JUMP_TIME = 0.3f;

    private Rigidbody2D _rb;

    private PlayerAnimationMethods _animationHandler;

    private IOverlapChecker _movementHelperClass;

    private Collider2D _col;

    private bool IS_JUMPING { get; set; } = false;

    private bool _isJumpPressed;

    private Vector3 _playerInitialPosition;

    public PlayerJumpTimeEvent onPlayerJumpTimeEvent;

    public CharacterVelocity CharacterVelocity { get; set; } = new CharacterVelocity();

    public Vector3 PlayerInitialPosition { get => _playerInitialPosition; set=> _playerInitialPosition = value; }

    public float TimeEclipsed { get; set; }

    private FlagDelegator FlagDelegator { get; set; }

    private PlayerVelocityDelegator PlayerVelocityDelegator { get; set; }

    public bool CancelAction()
    {
        FlagDelegator.NotifyObservers(false, gameObject.name, typeof(JumpingController), CancellationToken.None);

        return true;
    }
    public bool PerformAction(bool value)
    {
        _isJumpPressed = value;

        SetPlayerInitialPosition(IS_JUMPING);

        return true;
    }
    private void Awake()
    {
        _movementHelperClass = new MovementHelperClass();
        _col = GetComponent<CapsuleCollider2D>();
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _rb = GetComponent<Rigidbody2D>();

        FlagDelegator = Helper.GetDelegator<FlagDelegator>();

        PlayerVelocityDelegator = Helper.GetDelegator<PlayerVelocityDelegator>();
    }
    private void Start()
    {
        onPlayerJumpTimeEvent = new PlayerJumpTimeEvent(MAX_JUMP_TIME);

        onPlayerJumpTimeEvent.AddListener(MaxTimePassed);

        FlagDelegator.AddToSubjectsDict(typeof(JumpingController).ToString(), name, new Subject<IObserver<bool>>());

        FlagDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[name].SetSubject(this);

        PlayerVelocityDelegator.AddToSubjectsDict(typeof(JumpingController).ToString(), name, new Subject<IObserver<CharacterVelocity>>());

        PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[name].SetSubject(this);

    }
    private async void Update()
    {
        await HandleJumpingMechanism();

        if (IS_JUMPING && !PlayerSystem.IS_GRABBING)
        {
            TimeEclipsed += Time.deltaTime;
        }

        onPlayerJumpTimeEvent.ShouldFall(TimeEclipsed);
    }
    public async Task HandleJumpingMechanism()
    {
        await HandleJumping();

        await HandleFalling();

        PlayerVelocityDelegator.NotifyObservers(CharacterVelocity, gameObject.name, typeof(JumpingController), CancellationToken.None);

        await Task.FromResult(true);
    }

    public async Task HandleFalling()
    {
        if (await CanPlayerFall(maxJumpHeight) || !_isJumpPressed || onPlayerJumpTimeEvent.Fall) //falling
        {
            CharacterVelocity.VelocityY = -JumpSpeed * FALLING_SPEED;
        }

        if (!IsOnTheGround(groundLayer) && !IsOnTheLedge(ledgeLayer) && await IsYVelocityNegative(_rb))
        {
            _animationHandler.JumpingFallingAnimationHandler(false);
        }

        if ((IsOnTheGround(groundLayer) || IsOnTheLedge(ledgeLayer)) && !_isJumpPressed) //on the ground
        {
            IS_JUMPING = false;

            FlagDelegator.NotifyObservers(IS_JUMPING, gameObject.name, typeof(JumpingController), CancellationToken.None);

            onPlayerJumpTimeEvent.Fall = false;

            TimeEclipsed = 0;
        }

        await Task.FromResult(true);
    }

    public async Task HandleJumping()
    {
        if (await CanPlayerJump()) //jumping
        {
            IS_JUMPING = true;

            FlagDelegator.NotifyObservers(IS_JUMPING, gameObject.name, typeof(JumpingController), CancellationToken.None);

            CharacterVelocity.VelocityY = JumpSpeed * JUMPING_SPEED;

            _animationHandler.JumpingFallingAnimationHandler(true); 
        }

    }

    private Task<bool> CanPlayerJump()
    {
        bool isJumping = IS_JUMPING;
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

    public void OnNotifySubject(IObserver<bool> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        FlagDelegator.AddToSubjectObserversDict(gameObject.name, FlagDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[gameObject.name], observer);
    }

    public void OnNotifySubject(IObserver<CharacterVelocity> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        PlayerVelocityDelegator.AddToSubjectObserversDict(gameObject.name, PlayerVelocityDelegator.GetSubsetSubjectsDictionary(typeof(JumpingController).ToString())[gameObject.name], observer);

        StartCoroutine(PlayerVelocityDelegator.NotifyObserver(observer, new CharacterVelocity() { VelocityY = - 10f}, new NotificationContext() { SubjectType = typeof(JumpingController).ToString()}, cancellationToken));
    }
}
