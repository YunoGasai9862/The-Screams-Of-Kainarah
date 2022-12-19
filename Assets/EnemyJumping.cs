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

        if (JUMP && count <= 1f)
        {
            Debug.Log(count);
            rb.velocity = new Vector2(rb.velocity.x + 10f * Time.deltaTime, rb.velocity.y + 12f * Time.deltaTime );
            count += Time.deltaTime;


        }

        if(count>=1f)
        {
            count = 0f;
            rb.velocity = new Vector2(0, 0);
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
