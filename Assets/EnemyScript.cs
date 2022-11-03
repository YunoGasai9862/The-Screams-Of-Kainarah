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
    private GameObject Heroine;
    [SerializeField] bool StopForAttack = false;
    
    [SerializeField] LayerMask Player;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Heroine = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if(CanAttack())
        {
            StopForAttack = true;
            anim.SetBool("EnemyAttack", true);
        }
        else
        {
            anim.SetBool("EnemyAttack", false);
            StopForAttack = false;

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
        if(!StopForAttack) //continuing moving if the Player is not in the range
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[Index].position, Speed * Time.deltaTime);

        }

        if (Waypoints[Index].CompareTag("WP1"))
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
       
        if(StopForAttack)
        {
            transform.position=Vector2.MoveTowards(transform.position, Heroine.transform.position, Speed * Time.deltaTime);
        }

    }


    bool CanAttack()
    {


        if (sr.flipX)
        {

            Debug.DrawRay(transform.position, -transform.right * 1.5f, Color.cyan);
            return Physics2D.Raycast(transform.position, -transform.right, 1.5f, Player);

        }
        else
        {
            Debug.DrawRay(transform.position, transform.right * 1.5f, Color.cyan);

            return Physics2D.Raycast(transform.position, transform.right, 1.5f, Player); ;

        }

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
