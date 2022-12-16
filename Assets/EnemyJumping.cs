using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumping : MonoBehaviour
{

    //ENEMY JUMPING FROM ONE PLATFORM TO ANOTHER
    private Rigidbody2D rb;
    private Animator anim;
    private RaycastHit2D hit;
    private bool JUMP = false;
    [SerializeField] LayerMask Jumping;
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(transform.position, transform.right, .5f, Jumping);
        Debug.DrawRay(transform.position, transform.right * .5f, Color.red);
        if(hit.collider!=null && hit.collider.isTrigger)
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("CanWalk", false);
            JUMP = true;
            hit.collider.enabled = false;
           
        }

        if(JUMP)
        {
            rb.velocity = new Vector2(rb.velocity.x + 20f * Time.deltaTime, rb.velocity.y + 30f * Time.deltaTime);
            JUMP = false;
         
            
        }


       
    }

    private void FixedUpdate()
    {
        if(!JUMP)
        {
            rb.velocity = new Vector2(20 * Time.deltaTime, 0);
            if (rb.velocity.magnitude > .1f)
            {
                anim.SetBool("CanWalk", true);
            }
        }

      


    }

}
