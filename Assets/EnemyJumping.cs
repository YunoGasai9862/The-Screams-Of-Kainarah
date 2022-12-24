using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyJumping : MonoBehaviour
{

    //ENEMY JUMPING FROM ONE PLATFORM TO ANOTHER
    private Rigidbody2D rb;
    private Animator anim;
    private RaycastHit2D hit;
    private bool JUMP = false;
    private float count = 0;
    [SerializeField] LayerMask Jumping;
    [SerializeField] LayerMask Ledge;
    [SerializeField] LayerMask ground;
    private BoxCollider2D box;
    private Vector3 Scale;
    private float mulitplier = 1f;
    private Vector3 pos;
    private bool Climb = false;
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        Scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        if (GetComponent<SpriteRenderer>().flipX)
        {
            pos = transform.position;
            pos.x = transform.position.x - .3f;
            hit = Physics2D.Raycast(pos, -transform.right, .5f, Jumping);
            Debug.DrawRay(pos, -transform.right * .5f, Color.red);
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, transform.right, .5f, Jumping);
            Debug.DrawRay(transform.position, transform.right * .5f, Color.red);
        }

       
        if(hit.collider!=null && hit.collider.isTrigger && !hit.collider.CompareTag("Return") && !hit.collider.CompareTag("JumpBack"))
        {


            rb.velocity = new Vector2(0, 0);
            anim.SetBool("CanWalk", false);
            Scale.y = .92f;
            transform.localScale = Scale;
             JUMP = true;
            hit.collider.enabled = false;
           
        }

        if (JUMP && count <= 1f)
        {
            rb.AddForce(new Vector2(3f, 31f) * Time.deltaTime, ForceMode2D.Impulse);
           
            count += Time.deltaTime;



        }

        if(count>=1f)
        {
            count = 0f;
            JUMP = false;
            
           
         
        }

      
        if(isOntheGround())
        {
            Destroy(gameObject);

        }

        if (hit.collider != null && hit.collider.isTrigger && hit.collider.CompareTag("Return"))
        {
            rb.velocity = new Vector3(0, 0);
            GetComponent<SpriteRenderer>().flipX = true;
            mulitplier *= -1;
        }

        if(hit.collider != null && hit.collider.isTrigger && hit.collider.CompareTag("JumpBack"))
        {
            Climb = true;
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("CanWalk", false);
            hit.collider.enabled = false;
            Scale.y = .92f;
            transform.localScale = Scale;

        }

        if(Climb && count<=1f)
        {
          
            rb.AddForce(new Vector2(-3f,38f) * Time.deltaTime, ForceMode2D.Impulse);
            count += Time.deltaTime;

        }

        if(count>=1f)
        {
            count = 0f;
            Climb = false;
          
        }






    }

    private void FixedUpdate()
    {
        if(!Climb && !JUMP && isOntheLedge())
        {
            rb.velocity = new Vector2(mulitplier* 40 * Time.deltaTime, 0);
            if (rb.velocity.magnitude > .1f)
            {
                Scale.y = 1f;
                transform.localScale = Scale;
                anim.SetBool("CanWalk", true);
            }
        }

        

      


    }

    public bool isOntheLedge()
    {
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, .2f, Ledge);
    }

    bool isOntheGround()
    {
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, .2f, ground);
    }


}
