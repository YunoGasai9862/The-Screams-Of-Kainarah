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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startingGrav = rb.gravityScale;  //the initially gravity is stored in the array
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYSize),   //creates a box!
            0, ledge);
        //we dont need GreenYOffset* transform.localscale.y because the Y axis is fixed when rotating on X.axis, but we do need it for the X axis

        RedBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYoffset), new Vector2(redXSize, redYSize), 0 ,ledge);

         //if the variable is public static and exists on the same object, you can access it with the name of the script!!

        if(greenBox && !RedBox && !Movement.isGrabbing && !isOntheGround())
        {
            Movement.isGrabbing = true;
        }

        if(Movement.isGrabbing)
        {
            anim.SetBool("LedgeGrab", true);
        }
    }

    private void OnDrawGizmosSelected()  //drawing the boxes
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
