using GlobalAccessAndGameHelper;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LedgeGrab : MonoBehaviour, IReceiver<bool>
{
    private const float MAX_JUMP_HEIGHT_FROM_LEDGE_GRAB = 1f;
    private const float MAXIMUM_VELOCITY_Y_FORCE = 12f;
    private const float MAXIMUM_VELOCITY_X_FORCE = 12f;
    private const float FORCE = 30f;

    private bool greenBox, redBox;
    public float redXOffset, redYoffset, redXSize, redYSize, greenXOffset, greenYOffset, greenXsize, greenYSize;
    private MovementHelperClass _helperFunc;
    private Rigidbody2D rb;
    private float startingGrav;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask ledge;
    [SerializeField] float xDisplace, yDisplace;
    private CapsuleCollider2D col;
    private Animator anim;
    private SpriteRenderer sr;
    private float _timeSpent;
    private bool _canGrab = false;
    private Vector2 _groundPosition;
    public bool CanGrab { get => _canGrab; set => _canGrab = value; }
    public Vector2 GroundPositionBeforeLedgeGrab { get => _groundPosition; set => _groundPosition = value; }    
    private void Awake()
    {
        _helperFunc = new MovementHelperClass();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        startingGrav = rb.gravityScale;  //the initially gravity is stored in the array
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    async void Update()
    {

        greenXOffset = await GetBoxPosition(sr, greenXOffset);
        redXOffset = await GetBoxPosition(sr, redXOffset);

        if (!_helperFunc.overlapAgainstLayerMaskChecker(ref col, groundMask) && greenBox &&
            PlayerVariables.IS_GRABBING)
        {
            _timeSpent += Time.deltaTime;
        }

        if (TimeSpentGrabbing(_timeSpent, .3f) || _helperFunc.overlapAgainstLayerMaskChecker(ref col, ledge))
        {
            PlayerVariables.slideVariableEvent.Invoke(false);

            _timeSpent = 0f;
        }

        if (greenBox && !redBox && PlayerVariables.IS_JUMPING)
        {
            PlayerVariables.slideVariableEvent.Invoke(true);
        }

        //we dont need GreenYOffset* transform.localscale.y because the Y axis is fixed when rotating on X.axis, but we do need it for the X axis
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize), 0, ledge);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);
        //if the variable is public static and exists on the same object, you can access it with the name of the script!!

    }
    private async void FixedUpdate()
    {
        int sign = sr.flipX ? -1 : 1;
        await GrabLedge(sign, startingGrav, FORCE, GroundPositionBeforeLedgeGrab, new Vector2(MAXIMUM_VELOCITY_X_FORCE, MAXIMUM_VELOCITY_Y_FORCE));
    }

    //grab ledge => hold space until the player lands on the ledge
    public async Task HandleLedgeGrabCalculations(int sign, float startingGravity, float force, Vector2 groundPosition, Vector2 maximumVelocities)
    {
        if(rb.velocity.y < maximumVelocities.y)
            rb.AddForce(Vector2.up * yDisplace * force * Time.fixedDeltaTime * rb.mass, ForceMode2D.Impulse);
        if(transform.position.y > groundPosition.y + MAX_JUMP_HEIGHT_FROM_LEDGE_GRAB && rb.velocity.x < maximumVelocities.x)
            rb.AddForce((sign) * Vector2.right * xDisplace * force * Time.fixedDeltaTime * rb.mass, ForceMode2D.Impulse);
        rb.gravityScale = startingGravity;
        PlayerVariables.grabVariableEvent.Invoke(false);
        await Task.CompletedTask;
    }

    private void OnDrawGizmosSelected()//drawing the boxes (extras)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize));
    }
    private bool TimeSpentGrabbing(float timeSpent, float timeMargin)
    {
        return timeSpent > timeMargin;
    }

    public bool PerformAction(bool value)
    {
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0f;
        PlayerVariables.grabVariableEvent.Invoke(true);
        anim.SetBool("LedgeGrab", PlayerVariables.IS_GRABBING);
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
    private async Task GrabLedge(int sign, float startingGrav, float force, Vector2 groundPositionBeforeLedgeGrab, Vector2 maximumVelocities)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab")
           && CanGrab)
        {
            await HandleLedgeGrabCalculations(sign, startingGrav, force, groundPositionBeforeLedgeGrab, maximumVelocities);  //this is for setting the animation to false
            anim.SetBool("LedgeGrab", PlayerVariables.IS_GRABBING);
        }
    }

    //using in animations
    public Task StartLedgeGrab()
    {
        CanGrab = true;
        GroundPositionBeforeLedgeGrab = transform.position;
        return Task.CompletedTask;
    }
    public Task ShouldLedgeGrab()
    {
        CanGrab = false;
        return Task.CompletedTask;
    }
}
