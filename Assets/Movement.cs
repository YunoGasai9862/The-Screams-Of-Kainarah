using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed = 10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;
    [SerializeField] GameObject EnemyHitAnimation;


    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private float slidingspeed = 5f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(Horizontal * CharacterSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isOntheGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }


        if (Death)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

       
        Sliding();

        checkforFlip();

        CheckForAnimation();


    }
    void Sliding()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("Sliding", true);
            rb.velocity = new Vector2(slidingspeed, rb.velocity.y);
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


    void checkforFlip()
    {
        if (!Death)
        {
            if (Horizontal < 0f && isOntheGround() && flip)
            {
                sr.flipX = true;
                Vector2 offset = col.offset;
                offset.x += 1;
                col.offset = offset;
                flip = false;
            }
            else if (Horizontal > 0f && isOntheGround() && !flip)
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
            } else
            {
                anim.SetInteger("State", 0);

            }

            if (rb.velocity.y >= .1f)
            {
                anim.SetInteger("State", 2);

            } else if (rb.velocity.y <= -.1f)
            {
                anim.SetInteger("State", 3);
            }
        }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {

            GameObject HitAnim = Instantiate(EnemyHitAnimation, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(HitAnim, 3f);
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
                Death = true;
                anim.SetBool("Death", true);
            }
        }
    
}
