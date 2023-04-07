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
    [SerializeField] LayerMask Ledge;
    [SerializeField] GameObject DiamondHitEffect;
    [SerializeField] GameObject pickupEffect;

    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private float slidingspeed = 5f;
    public static double MAXHEALTH=100f;
    public static double ENEMYATTACK = 5f;
    [SerializeField] GameObject TeleportTransition;

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
                rb.velocity = new Vector2(Horizontal * CharacterSpeed, rb.velocity.y);

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


        if (rb.velocity.y < -15f && (!isOntheGround() || !isontheLedge())) //freefalling into an abyss. Not a good solution, i know
        {
            GameStateManager.RestartGame();
        }

  

        checkforFlip();


        Sliding();


        CheckForAnimation();
        
       



    }
    private void FixedUpdate()
    {
        if(checkForExistenceOfPortal(sr))
        {

            Instantiate(TeleportTransition, transform.position, Quaternion.identity);
            StartCoroutine(WaiterFunction());
            GameStateManager.ChangeLevel(SceneManager.GetActiveScene().buildIndex);
        }
    }



    void Sliding()
    {


        if (Input.GetKeyDown(KeyCode. LeftShift) )
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
                Vector2 offset = col.offset;
                offset.x += 1;
                col.offset = offset;



                flip = false;
            }
            else if (Horizontal > 0f && (isOntheGround() || isontheLedge()) && !flip)
            {

                sr.flipX = false;

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





  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || (collision.collider.CompareTag("Boss") &&
            (collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack") ||
            collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_02"))))
        {
            if(MAXHEALTH==0)
            {
                Death = true;
                anim.SetBool("Death", true);
            }
            else
            {
                MAXHEALTH -= ENEMYATTACK;
            }


           
        }

        

      

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crystal"))
        {
            GameObject DHE = Instantiate(DiamondHitEffect, collision.transform.position, Quaternion.identity);
            Destroy(DHE, 2f);
            
        }

        if (collision.CompareTag("Health"))
        {
           
            if(MAXHEALTH<100)
            {
                MAXHEALTH += 10;
                GameObject temp = Instantiate(pickupEffect, collision.gameObject.transform.position, Quaternion.identity);
                Destroy(collision.gameObject);
                Destroy(temp, 1f);
            }

        
        }
    }

    public bool checkForExistenceOfPortal(SpriteRenderer sr)
    {
        RaycastHit hit; //using 3D raycast because of 3D object, portal
        Vector2 pos=transform.position;
        if(sr.flipX)
        {
            pos.x = transform.position.x - 1f;
            Debug.DrawRay(pos, -transform.right *1, Color.red);
            Physics.Raycast(transform.position, -transform.right, out hit, 1f);
          
          

        }
        else
        {
            pos.x = transform.position.x + 1f;

            Debug.DrawRay(transform.position, transform.right * 1, Color.red);

            Physics.Raycast(transform.position, -transform.right, out hit, 1f);


        }
        if(hit.collider!=null)

            Debug.Log(hit.collider.name);
      

        if (hit.collider!=null && hit.collider.isTrigger && hit.collider.CompareTag("Portal"))
        {
            return true;
        }

        return false;


    }

    IEnumerator WaiterFunction()
    {
        yield return new WaitForSeconds(1f);
    }











}


     

    

        

    
