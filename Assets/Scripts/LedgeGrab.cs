using GlobalAccessAndGameHelper;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LedgeGrab : MonoBehaviour, IReceiver
{
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
    private const float MAX_JUMP_HEIGHT_FROM_LEDGE_GRAB = 1f;
    private bool _canGrab = false;
    private Vector2 _groundPosition;
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
    void Update()
    {
        if (sr.flipX)
        {
            greenXOffset = -.1f;
            redXOffset = -0.1f;

        }
        else
        {
            greenXOffset = 0.09f;
            redXOffset = .1f;
        }

        if (!_helperFunc.overlapAgainstLayerMaskChecker(ref col, groundMask) && greenBox &&
            PlayerActionRelayer.isGrabbing)
        {
            _timeSpent += Time.deltaTime;
        }

        if (TimeSpentGrabbing(_timeSpent, .3f) || _helperFunc.overlapAgainstLayerMaskChecker(ref col, ledge))
        {
            PlayerActionRelayer.isGrabbing = false;
            _timeSpent = 0f;
        }


        //we dont need GreenYOffset* transform.localscale.y because the Y axis is fixed when rotating on X.axis, but we do need it for the X axis
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize), 0, ledge);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);
        //if the variable is public static and exists on the same object, you can access it with the name of the script!!

        if (greenBox && !redBox && globalVariablesAccess.ISJUMPING)
        {
            PlayerActionRelayer.isGrabbing = true;
        }

    }

    private async void FixedUpdate()
    {
       await GrabLedge();
    }

    public async Task HandleLedgeGrabCalculations(int sign, float startingGravity, float force, Vector2 groundPosition)
    {
        rb.AddForce(Vector2.up * yDisplace * force * Time.fixedDeltaTime * rb.mass, ForceMode2D.Impulse);
        Debug.Log(groundPosition);
        if(transform.position.y > groundPosition.y + MAX_JUMP_HEIGHT_FROM_LEDGE_GRAB)
            rb.AddForce((sign) * Vector2.right * xDisplace * force * Time.fixedDeltaTime * rb.mass, ForceMode2D.Impulse);
        
        rb.gravityScale = startingGravity;
        PlayerActionRelayer.isGrabbing = false;
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

    public void PerformAction()
    {
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0f;
        anim.SetBool("LedgeGrab", true);
    }

    public void CancelAction()
    {
        CancelLedgeGrab();
    }

    private Task CancelLedgeGrab()
    {
        return Task.CompletedTask;
    }
    private async Task GrabLedge()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab")
           && _canGrab)
        {
            int sign = sr.flipX ? -1 : 1;
            await HandleLedgeGrabCalculations(sign, startingGrav, 100, _groundPosition);  //this is for setting the animation to false
            anim.SetBool("LedgeGrab", false);
        }
    }

    public Task StartLedgeGrab()
    {
        _canGrab = true;
        _groundPosition = transform.position;
        return Task.CompletedTask;
    }

    public Task ShouldLedgeGrab()
    {
        _canGrab = false;
        return Task.CompletedTask;
    }

}
