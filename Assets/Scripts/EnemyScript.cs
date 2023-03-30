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
    public static RaycastHit2D[] hit;
    
    [SerializeField] LayerMask Player;
    [SerializeField] GameObject Hit;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Heroine = GameObject.FindWithTag("Player");
        if(transform.gameObject.name!="Enemy2")
        {
            anim.SetBool("Destroyed", false);
        }
       
    }

    void Update()
    {
        if(transform.gameObject.name!="Enemy2")
        {
            if (CanAttack())
            {
                StopForAttack = true;
                rb.velocity = new Vector2(0, 0);

                if(sr.flipX)
                {
                    anim.SetBool("EnemyAttack2", true);

                }
                else
                {
                    anim.SetBool("EnemyAttack", true);

                }


            }
            else
            {
                if (sr.flipX)
                {
                    anim.SetBool("EnemyAttack2", false);

                }
                else
                {
                    anim.SetBool("EnemyAttack", false);

                }

                StopForAttack = false;

            }

            
        }

        if (lifeCounter > 3)
        {
            isNotdead = false;
            Destroy(gameObject, .5f);
            lifeCounter = 0;
        }




        if (Vector2.Distance(transform.position, Waypoints[Index].position)<.1f)
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


    }


    bool CanAttack()
    {


        if (sr.flipX)
        {

            Debug.DrawRay(transform.position, -transform.right * 3f, Color.cyan);
            return Physics2D.Raycast(transform.position, -transform.right, 3f, Player);

        }
        else
        {
            Debug.DrawRay(transform.position, transform.right * 3f, Color.cyan);

            return Physics2D.Raycast(transform.position, transform.right, 3f, Player);

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger") || collision.CompareTag("Sword"))
        {
            if(isNotdead)
            {
                GameObject endofLife = Instantiate(Hit, transform.position, Quaternion.identity);

                if (transform.gameObject.name != "Enemy2")
                {
                    anim.SetBool("Hit", true);
                }

                lifeCounter++;
                Destroy(endofLife, 1f);

            }


        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger") || collision.CompareTag("Sword"))
        {

            if (transform.gameObject.name != "Enemy2")
            {
                anim.SetBool("Hit", false);
            }

        }
    }

}
