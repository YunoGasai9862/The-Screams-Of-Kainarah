using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    private bool greenBox, RedBox;
    public float redXOffset, redYoffset, redXSize, redYSize, greenXOffset, greenYOffset, greenXsize, greenYSize;
    private Rigidbody2D rb;
    private float startingGrav;
    [SerializeField] LayerMask groundmask;
    [SerializeField] LayerMask ledge;
    private BoxCollider2D col;
    private Animator anim;
    private SpriteRenderer sr;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        startingGrav = rb.gravityScale;  //the initially gravity is stored in the array
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if(sr.flipX)
        {
            greenXOffset = -.1f;
            redXOffset = -0.1f;
           
        }
        else
        {
            greenXOffset = 0.09f;
            redXOffset = .1f;
           

        }


        //we dont need GreenYOffset* transform.localscale.y because the Y axis is fixed when rotating on X.axis, but we do need it for the X axis
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize)
            ,0, ledge);
        RedBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0, ledge);
        //if the variable is public static and exists on the same object, you can access it with the name of the script!!

         if(greenBox && !RedBox && !Movement.isGrabbing && !isOntheGround())
        {
            Movement.isGrabbing = true;
        }
        if(Movement.isGrabbing)
        {
            anim.SetBool("LedgeGrab", true);
            rb.velocity = new Vector2(0, 0);//setting the x and y velocity to zero  (even i was doing the same in my implementation!)
            rb.gravityScale = 0f;  //and sets the gravity scale to zero
        }
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=.5f) //it checks for the same animations normalziaed TIME!!
        {
            ChangePositionOfThePlayer();  //this is for setting the animation to false
            anim.SetBool("LedgeGrab", false);

        }
    }

    public void ChangePositionOfThePlayer()
    {
        if(sr.flipX)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y +12 * Time.deltaTime * transform.localScale.y);
            transform.position = new Vector2(transform.position.x - 10 * Time.deltaTime * transform.localScale.x, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 12 * Time.deltaTime * transform.localScale.y);
            transform.position = new Vector2(transform.position.x + 10 * Time.deltaTime * transform.localScale.x, transform.position.y);
        }
       
        rb.gravityScale = startingGrav;
        Movement.isGrabbing = false;

    }

    private void OnDrawGizmosSelected()  //drawing the boxes (extras)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize));
    }

    bool isOntheGround()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, groundmask);
    }
}
