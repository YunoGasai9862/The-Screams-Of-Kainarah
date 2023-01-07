using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed = 10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;
    [SerializeField] AttackEnemy Enemy;
    [SerializeField] LayerMask Ledge;

    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private float slidingspeed = 5f;
    public static double MAXHEALTH=100f;
    public static double ENEMYATTACK = 10f;

    public static bool isGrabbing = false;//for the ledge grab script

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        MAXHEALTH = 100f;

    }
    void Update()
    {

        if (!isGrabbing && !anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab")) 
        {
            Horizontal = Input.GetAxisRaw("Horizontal");

            if (rb.bodyType != RigidbodyType2D.Static)
            {
                rb.velocity = new Vector3(Horizontal * CharacterSpeed, rb.velocity.y);

            }

        }


        if (Input.GetButtonDown("Jump") && (isOntheGround() || isontheLedge()))
        {
            if (rb.bodyType != RigidbodyType2D.Static)
                rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }

        if (Death)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }





        checkforFlip();


        Sliding();


        CheckForAnimation();




    }

  

    void Sliding()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift) )
        {
            anim.SetBool("Sliding", true);
          
            if(sr.flipX)
            {
                rb.velocity = new Vector2(-slidingspeed, rb.velocity.y);

            }else
            {
                rb.velocity = new Vector2(slidingspeed, rb.velocity.y);

            }


        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anim.SetBool("Sliding", false);
        }

    }
    bool isOntheGround()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, Ground);
    }

    bool isontheLedge()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, Ledge);
    }
    void checkforFlip()
    {
        if (!Death)
        {
            if (Horizontal < 0f && (isOntheGround() || isontheLedge()) && flip)
            {
                //character flip

                sr.flipX = true;
                Enemy.HeroineFlipped = true;
                Vector2 offset = col.offset;
                offset.x += 1;
                col.offset = offset;



                flip = false;
            }
            else if (Horizontal > 0f && (isOntheGround() || isontheLedge()) && !flip)
            {

                sr.flipX = false;
                Enemy.HeroineFlipped = false;

                Vector2 offset = col.offset;
                offset.x -= 1;

                col.offset = offset;
                flip = true;
            }
        }

    }


    void CheckForAnimation()
    {
        if (Horizontal > 0f || Horizontal < 0f)
        {
            anim.SetInteger("State", 1);
        }
        else
        {
            anim.SetInteger("State", 0);

        }

        if (rb.velocity.y >= .1f)
        {
            anim.SetInteger("State", 2);

        }
        else if (rb.velocity.y <= -.1f)
        {
            anim.SetInteger("State", 3);
        }
    }





    void Restart()
    {

        rb.bodyType = RigidbodyType2D.Static;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            if(MAXHEALTH==0)
            {
                Death = true;
                anim.SetBool("Death", true);
            }
            else
            {
                MAXHEALTH -= ENEMYATTACK;
                Debug.Log(MAXHEALTH);
            }


           
        }
    }


   




        
    
}


     

    

        

    
