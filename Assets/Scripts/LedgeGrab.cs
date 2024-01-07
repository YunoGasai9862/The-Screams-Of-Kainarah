using GlobalAccessAndGameHelper;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class LedgeGrab : MonoBehaviour, IReceiver
{
    private bool greenBox, redBox;
    public float redXOffset, redYoffset, redXSize, redYSize, greenXOffset, greenYOffset, greenXsize, greenYSize;
    private MovementHelperClass _helperFunc;
    private Rigidbody2D rb;
    private float startingGrav;
    [SerializeField] LayerMask groundmask;
    [SerializeField] LayerMask ledge;
    [SerializeField] float Xdisplace, Ydisplace;
    private CapsuleCollider2D col;
    private Animator anim;
    private SpriteRenderer sr;
    private float _timeSpent;

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

        if (!_helperFunc.overlapAgainstLayerMaskChecker(ref col, groundmask) && greenBox &&
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

        if (PlayerActionRelayer.isGrabbing)
        {
            anim.SetBool("LedgeGrab", true);
        }

    }

    private async void FixedUpdate()
    {
        //worked here, fix it tomorrow
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .2f) //it checks for the same animations normalziaed TIME!!
        {
            int sign = sr.flipX ? -1 : 1;
            await ChangePositionOfThePlayer(sign, startingGrav, 10000);  //this is for setting the animation to false
            anim.SetBool("LedgeGrab", false);

        }
    }

    public async Task ChangePositionOfThePlayer(int sign, float startingGravity, float force)
    {
        rb.AddForce(new Vector2((sign) * Xdisplace * force, Ydisplace * force) * rb.mass, ForceMode2D.Impulse);
        //transform.position = new Vector2(transform.position.x, transform.position.y + Ydisplace * Time.deltaTime * transform.localScale.y);
        //transform.position = new Vector2(transform.position.x + (sign) * Xdisplace * Time.deltaTime * transform.localScale.x, transform.position.y);   
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
        GrabLedge();
    }

    public void CancelAction()
    {
        CancelLedgeGrab();
    }

    private Task CancelLedgeGrab()
    {
        return Task.CompletedTask;
    }
    private Task GrabLedge()
    {
        return Task.CompletedTask;

    }

}
