using Assets.Annotations;
using System;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Threading.Tasks;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerAttributesNotifier), EntityType = typeof(PlayerLedgeGrabController), ContextType = typeof(Player))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerStateConsumer), EntityType = typeof(PlayerLedgeGrabController), ContextType = typeof(GenericStateBundle<PlayerStateBundle>))]
public class PlayerLedgeGrabController : MonoBehaviour, IReceiverEnhancedAsync<PlayerLedgeGrabController, PlayerStateBundle>, INotify<Player>, INotify<GenericStateBundle<PlayerStateBundle>>
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

    private float startingGrav;

    private float _timeSpent;

    private bool _canGrab = false;

    private Vector2 _groundPosition;

    private bool CanGrab { get => _canGrab; set => _canGrab = value; }

    private Vector2 GroundPositionBeforeLedgeGrab { get => _groundPosition; set => _groundPosition = value; }

    private bool StartCalculatingGrabLedgeDisplacement { get; set; }

    private GenericStateBundle<PlayerStateBundle> PlayerBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>() { StateBundle = new PlayerStateBundle() };

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private Delegator Delegator { get; set; }

    private Player Player { get; set; }

    private async void Awake()
    {
        _helperFunc = new MovementHelperClass();

       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        PlayerStateEvent = await SceneUtils.GetCustomEvent<PlayerStateEvent>();

        if (Delegator == null)
        {
            throw new DelegatorNotFoundException("Delegator not found!!");
        }

        if (PlayerStateEvent == null)
        {
            throw new CustomEventNotFoundException("PlayerStateEvent not found!!");
        }
    }
    public void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerLedgeGrabController),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<GenericStateBundle<PlayerStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerLedgeGrabController),
            SubjectType = typeof(PlayerStateConsumer)
        }, this));

        ledgradeAnimationEvent.AddListener(LedgeGrabEventAnimationKeeperListener);
    }

    //SNCE ITS A CONTROLLER - IT SHOULD NOT BE RNNNING IN UPDATE!! FIX IT!

    async void Update()
    {
        if (Player == null)
        {
            Debug.Log($"Player is null is the LedgeGrabController - skipping async Update!");
            return;
        }

        greenBox = Physics2D.OverlapBox(new Vector2(Player.Transform.position.x + (await GetBoxPosition(Player.SpriteRendererValue.Renderer, greenXOffset)), Player.Transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize), 0, ledge);

        redBox = Physics2D.OverlapBox(new Vector2(Player.Transform.position.x + (await GetBoxPosition(Player.SpriteRendererValue.Renderer, redXOffset)), Player.Transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);

        if (!_helperFunc.OverlapAgainstLayerMaskChecker(Player.Collider, groundMask, COLLIDER_DISTANCE_FROM_THE_LAYER) && greenBox &&
            PlayerBundle.StateBundle.PlayerMovementState.Equals(ActionState.IS_GRABBING))
        {
            _timeSpent += Time.deltaTime;
        }

        if(_helperFunc.OverlapAgainstLayerMaskChecker(Player.Collider, groundMask, COLLIDER_DISTANCE_FROM_THE_LAYER) || _helperFunc.OverlapAgainstLayerMaskChecker(Player.Collider, ledge, COLLIDER_DISTANCE_FROM_THE_LAYER))
        {
            _timeSpent = 0f;

            PlayerBundle.StateBundle.PlayerLeapState = new State<LeapState, bool> { CurrentState = LeapState.IS_FALLING, CurrentValue = false, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerBundle);

        }

        if (greenBox && !redBox && !TimeSpentGrabbing(_timeSpent, MAX_TIME_FOR_LEDGE_GRAB)  && PlayerBundle.StateBundle.PlayerLeapState.CurrentState != LeapState.IS_FALLING)
        {
            PlayerBundle.StateBundle.PlayerActionState = new State<ActionState, bool> { CurrentState = ActionState.IS_GRABBING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerBundle);

            Player.Collider.isTrigger = true;

            //look for better way to make isConcluded compatible etc

            Player.Animator.SetBool(PlayerAnimationField.LedgeGrab.ToString(), !PlayerBundle.StateBundle.PlayerActionState.IsConcluded);

        }else
        {
            PlayerBundle.StateBundle.PlayerActionState = new State<ActionState, bool> { CurrentState = ActionState.IS_GRABBING, CurrentValue = false, IsConcluded = true };
            
            await PlayerStateEvent.Invoke(PlayerBundle);

            Player.Collider.isTrigger = false;

            Player.Animator.SetBool(PlayerAnimationField.LedgeGrab.ToString(), !PlayerBundle.StateBundle.PlayerActionState.IsConcluded);

            Player.Rigidbody.gravityScale = startingGrav;
        }

    }
    private async void FixedUpdate()
    {
        if (Player == null)
        {
            Debug.Log($"Player is null is the LedgeGrabController - skipping async FixedUpdate!");
            return;
        }

        int sign = await SceneUtils.PlayerFlipped(Player.Transform);

        await GrabLedge(Player.Animator, Player.Rigidbody);

        if(StartCalculatingGrabLedgeDisplacement)
        {
            await HandleLedgeGrabCalculations(sign, ledgeGrabForces, new Vector2(0, MAXIMUM_VELOCITY_Y_FORCE));

            PlayerBundle.StateBundle.PlayerLeapState = new State<LeapState, bool>() { CurrentState = LeapState.IS_FALLING, CurrentValue = true, IsConcluded = false };

            await PlayerStateEvent.Invoke(PlayerBundle);

            await SetGravityValue(Player.Rigidbody, startingGrav);

            StartCalculatingGrabLedgeDisplacement = false;
        }
    }
    public async Task HandleLedgeGrabCalculations(int sign, Vector2 force, Vector2 maximumVelocities)
    {
        if (Player.Rigidbody.linearVelocity.y < maximumVelocities.y)
        {
            Player.Rigidbody.AddForce(Vector2.up * displacements.y * force.y * Player.Rigidbody.mass, ForceMode2D.Impulse);
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
        if (Player.Animator == null || Player.Rigidbody == null)
        {
            Debug.Log($"Animator or Rigidbody is null in Ledge Grab Controller - skipping GrabLedge!");
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(PlayerAnimationField.LedgeGrab.ToString())
           && CanGrab)
        {
            await SetGravityValue(rb, 0f);

            PlayerBundle.StateBundle.PlayerActionState = new State<ActionState, bool>() { CurrentState = ActionState.IS_GRABBING, CurrentValue = false, IsConcluded = true };

            await PlayerStateEvent.Invoke(PlayerBundle);

            anim.SetBool(PlayerAnimationField.LedgeGrab.ToString(), !PlayerBundle.StateBundle.PlayerActionState.IsConcluded);
        }
    }

    private void LedgeGrabEventAnimationKeeperListener(bool value)
    {
        StartCalculatingGrabLedgeDisplacement = true;

        CanGrab = false;
    }
    
    public Task StartLedgeGrab()
    {
        CanGrab = true;
        GroundPositionBeforeLedgeGrab = transform.position;
        return Task.CompletedTask;
    }

    public async Task<ActionExecuted> PerformAction(PlayerStateBundle value)
    {
        //WTF!!!
        //Player.Rigidbody.linearVelocity = new Vector2(0, 0);

        return await Task.FromResult(new ActionExecuted() { Result = true });
    }

    public async Task<ActionExecuted> CancelAction(PlayerStateBundle value)
    {
        await CancelLedgeGrab();

        return await Task.FromResult(new ActionExecuted() { Result = false });
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        startingGrav = Player.Rigidbody.gravityScale;

        yield return null;
    }

    public IEnumerator Notify(GenericStateBundle<PlayerStateBundle> value)
    {
        PlayerBundle.StateBundle = value.StateBundle;

        yield return null;
    }
}
