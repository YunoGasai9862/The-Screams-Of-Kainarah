using GlobalAccessAndGameHelper;
using System;
using UnityEngine;

public class LedgeGrab : MonoBehaviour, IReceiver
{
    private bool greenBox, RedBox;
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

        if (!_helperFunc.overlapAgainstLayerMaskChecker(ref col, groundmask) && greenBox &&
            PlayerActionRelayer.isGrabbing)
        {
            _timeSpent += Time.deltaTime;
        }

        if (TimeSpentIsGrabbing() || _helperFunc.overlapAgainstLayerMaskChecker(ref col, ledge))
        {
            PlayerActionRelayer.isGrabbing = false;
            _timeSpent = 0f;
        }


        //we dont need GreenYOffset* transform.localscale.y because the Y axis is fixed when rotating on X.axis, but we do need it for the X axis
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize), 0, ledge);
        RedBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);
        //if the variable is public static and exists on the same object, you can access it with the name of the script!!

        if (greenBox && !RedBox && globalVariablesAccess.ISJUMPING)
        {
            PlayerActionRelayer.isGrabbing = true;
        }

        if (PlayerActionRelayer.isGrabbing)
        {
            anim.SetBool("LedgeGrab", true);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .2f) //it checks for the same animations normalziaed TIME!!
        {
            ChangePositionOfThePlayer();  //this is for setting the animation to false
            anim.SetBool("LedgeGrab", false);

        }
    }

    public void ChangePositionOfThePlayer()
    {
        if (sr.flipX)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Ydisplace * Time.deltaTime * transform.localScale.y);
            transform.position = new Vector2(transform.position.x - Xdisplace * Time.deltaTime * transform.localScale.x, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Ydisplace * Time.deltaTime * transform.localScale.y);
            transform.position = new Vector2(transform.position.x + Xdisplace * Time.deltaTime * transform.localScale.x, transform.position.y);
        }

        rb.gravityScale = startingGrav;
        PlayerActionRelayer.isGrabbing = false;

    }

    private void OnDrawGizmosSelected()//drawing the boxes (extras)
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize));

    }
    private bool TimeSpentIsGrabbing()
    {
        return _timeSpent > .3f;
    }

    public void PerformAction()
    {
        GrabLedge();
    }

    public void CancelAction()
    {
        CancelLedgeGrab();
    }

    private void CancelLedgeGrab()
    {
        Debug.Log("Not Grabbing Ledge");

    }
    private void GrabLedge()
    {
        Debug.Log("Grabbing Ledge");
    }

}
