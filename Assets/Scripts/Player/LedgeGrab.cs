using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LedgeGrab : MonoBehaviour, IObserver<GenericStateBundle<PlayerStateBundle>>, IReceiver<bool>
{
    private const float MAXIMUM_VELOCITY_Y_FORCE = 12f;

    private const float MAX_TIME_FOR_LEDGE_GRAB = 1f;

    private const float COLLIDER_DISTANCE_FROM_THE_LAYER = 0.05f;

    private const float VELOCITY_ASYNC_DELAY = 0.15f;

    [SerializeField] LayerMask groundMask;

    [SerializeField] LayerMask ledge;

    [SerializeField] Vector2 displacements;

    [SerializeField] Vector2 ledgeGrabForces;

    [SerializeField] LedgeGrabAnimationEvent ledgradeAnimationEvent;

    private bool greenBox, redBox;

    public float redXOffset, redYoffset, redXSize, redYSize, greenXOffset, greenYOffset, greenXsize, greenYSize;

    private MovementHelperClass _helperFunc;

    private Rigidbody2D rb;

    private float startingGrav;

    private Collider2D col;

    private Animator anim;

    private SpriteRenderer sr;

    private float _timeSpent;

    private bool _canGrab = false;

    private Vector2 _groundPosition;

    private bool CanGrab { get => _canGrab; set => _canGrab = value; }

    private Vector2 GroundPositionBeforeLedgeGrab { get => _groundPosition; set => _groundPosition = value; }

    private bool StartCalculatingGrabLedgeDisplacement { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private void Awake()
    {
        _helperFunc = new MovementHelperClass();

        PlayerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = Helper.GetCustomEvent<PlayerStateEvent>();

        if (PlayerStateDelegator == null)
        {
            throw new DelegatorNotFoundException("PlayerStateDelegator not found!!");
        }

        if (PlayerStateEvent == null)
        {
            throw new CustomEventNotFoundException("PlayerStateEvent not found!!");
        }
    }
    void Start()
    {
        StartCoroutine(PlayerStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None));


        rb = GetComponent<Rigidbody2D>();

        col = GetComponent<CapsuleCollider2D>();

        startingGrav = rb.gravityScale;  //the initially gravity is stored in the array

        anim = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        ledgradeAnimationEvent.AddListener(LedgeGrabEventAnimationKeeperListener);
    }
    // Update is called once per frame
    async void Update()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (await GetBoxPosition(sr, greenXOffset)), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize), 0, ledge);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (await GetBoxPosition(sr, redXOffset)), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);

        if (!_helperFunc.OverlapAgainstLayerMaskChecker(ref col, groundMask, COLLIDER_DISTANCE_FROM_THE_LAYER) && greenBox &&
            PlayerBundle.StateBundle.PlayerMovementState.Equals(PlayerActionState.IS_GRABBING))
        {
            _timeSpent += Time.deltaTime;
        }

        if(_helperFunc.OverlapAgainstLayerMaskChecker(ref col, groundMask, COLLIDER_DISTANCE_FROM_THE_LAYER) || _helperFunc.OverlapAgainstLayerMaskChecker(ref col, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            _timeSpent = 0f;

            PlayerBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState> { CurrentState = PlayerMovementState.IS_FALLING, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerBundle);

        }

        if (greenBox && !redBox && !TimeSpentGrabbing(_timeSpent, MAX_TIME_FOR_LEDGE_GRAB)  && PlayerBundle.StateBundle.PlayerMovementState.CurrentState != PlayerMovementState.IS_FALLING)
        {
            PlayerBundle.StateBundle.PlayerActionState = new State<PlayerActionState> { CurrentState = PlayerActionState.IS_GRABBING, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerBundle);

            col.isTrigger = true;

            //look for better way to make isConcluded compatible etc

            anim.SetBool(PlayerAnimationConstants.LEDGE_GRAB, !PlayerBundle.StateBundle.PlayerActionState.IsConcluded);

        }else
        {
            PlayerBundle.StateBundle.PlayerActionState = new State<PlayerActionState> { CurrentState = PlayerActionState.IS_GRABBING, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerBundle);

            col.isTrigger = false;

            anim.SetBool(PlayerAnimationConstants.LEDGE_GRAB, !PlayerBundle.StateBundle.PlayerActionState.IsConcluded); 

            rb.gravityScale = startingGrav;
        }

    }
    private async void FixedUpdate()
    {
        int sign = await Helper.PlayerFlipped(transform);

        await GrabLedge(anim, rb);

        if(StartCalculatingGrabLedgeDisplacement)
        {
            await HandleLedgeGrabCalculations(sign, ledgeGrabForces, new Vector2(0, MAXIMUM_VELOCITY_Y_FORCE));

            PlayerBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_FALLING, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerBundle);

            await SetGravityValue(rb, startingGrav);

            StartCalculatingGrabLedgeDisplacement = false;
        }
    }
    public async Task HandleLedgeGrabCalculations(int sign, Vector2 force, Vector2 maximumVelocities)
    {
        if (rb.linearVelocity.y < maximumVelocities.y)
        {
            rb.AddForce(Vector2.up * displacements.y * force.y * rb.mass, ForceMode2D.Impulse);
        }

        await Task.Delay(TimeSpan.FromSeconds(VELOCITY_ASYNC_DELAY));
    }

    private void OnDrawGizmosSelected()//drawing the boxes (extras)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize));
    }

    private async Task SetGravityValue(Rigidbody2D rb, float value)
    {
        rb.gravityScale = value;

        await Task.FromResult(true);
    }
    private bool TimeSpentGrabbing(float timeSpent, float timeMargin)
    {
        return timeSpent > timeMargin;
    }

    public bool PerformAction(bool value)
    {
        rb.linearVelocity = new Vector2(0, 0);

        return true;
    }

    public bool CancelAction()
    {
        CancelLedgeGrab();

        return true;
    }

    private Task<float> GetBoxPosition(SpriteRenderer sr, float currentValue)
    {
        return sr.flipX? Task.FromResult(-1f * currentValue) : Task.FromResult(currentValue);
    }
    private Task CancelLedgeGrab()
    {
        return Task.CompletedTask;
    }
    private async Task GrabLedge(Animator anim, Rigidbody2D rb)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(PlayerAnimationConstants.LEDGE_GRAB)
           && CanGrab)
        {
            await SetGravityValue(rb, 0f);

            PlayerBundle.StateBundle.PlayerActionState = new State<PlayerActionState>() { CurrentState = PlayerActionState.IS_GRABBING, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerBundle);

            anim.SetBool(PlayerAnimationConstants.LEDGE_GRAB, !PlayerBundle.StateBundle.PlayerActionState.IsConcluded);
        }
    }

    private void LedgeGrabEventAnimationKeeperListener(bool value)
    {
        StartCalculatingGrabLedgeDisplacement = true;

        CanGrab = false;
    }
    
    //using in animations
    public Task StartLedgeGrab()
    {
        CanGrab = true;
        GroundPositionBeforeLedgeGrab = transform.position;
        return Task.CompletedTask;
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerBundle.StateBundle = data.StateBundle;
    }
}
