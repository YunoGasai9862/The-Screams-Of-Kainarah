using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] Transform[] Waypoints;
    private int Index=0;
    private Animator anim;
    private int lifeCounter = 0;
    private bool isNotdead = true;
    private float Speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    [SerializeField] LayerMask Player;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(CanAttack())
        {
            anim.SetBool("EnemyAttack", true);
        }
        else
        {
            anim.SetBool("EnemyAttack", false);

        }

        if(lifeCounter>=3)
        {
            isNotdead = false;
            Destroy(gameObject,.3f);
        }


        if(Vector2.Distance(transform.position, Waypoints[Index].position)<=.1)
        {
            Index++;

            if(Index>=Waypoints.Length)
            {
                Index = 0;
            }

        }
            transform.position=Vector3.MoveTowards(transform.position, Waypoints[Index].position, Speed * Time.deltaTime);

        if (Waypoints[Index].CompareTag("WP1"))
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
       

    }


    bool CanAttack()
    {

    
       if(sr.flipX)
        {
          return Physics2D.Raycast(transform.position, -transform.right, 3f, Player);
          
        }

        return Physics2D.Raycast(transform.position, transform.right, 3f, Player); ;
      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            if(isNotdead)
            {
                anim.SetBool("Hit", true);
                lifeCounter++;
            }
          

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            anim.SetBool("Hit", false );

        }
    }

}
