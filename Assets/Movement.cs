using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed=10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject EnemyHitAnimation;


    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private int AttackCount = 1;
    private float slidingspeed=5f;

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

        rb.velocity = new Vector3(Horizontal * CharacterSpeed, rb.velocity.y);

        if(Input.GetButtonDown("Jump") && isOntheGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }

         
    
        if(Death)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        Sliding();

        checkforFlip();

        CheckForAnimation();

        AttackingMechanism();
          
        
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
        if(!Death)
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

    void AttackingMechanism()
    {

        if (!isOntheGround() && Input.GetMouseButtonDown(0))
        {
            anim.SetBool("AttackJ", true);
        }
        else
        {
            anim.SetBool("AttackJ", false);
        }

        if (Input.GetMouseButtonDown(0))
        {

            anim.SetBool("Attack", true);
              
            anim.SetInteger("AttackCount", AttackCount);
            AttackCount++;

            
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime>1)
            {

            }

            if (AttackCount > 4)
            {
                anim.SetBool("Attack", false);
                AttackCount = 1;
            }

            
            if (CheckRangeForDestroyEnemy())
            {
                GameObject HitAnim = Instantiate(EnemyHitAnimation, Enemy.transform.position, Quaternion.identity);
                Destroy(Enemy.gameObject);
                Destroy(HitAnim, 3f);
            }


        }
    
    }
    void CheckForAnimation()
    {
        if(Horizontal >0f || Horizontal <0f)
        {
            anim.SetInteger("State", 1);
        }else
        {
            anim.SetInteger("State", 0);

        }

        if(rb.velocity.y >=.1f)
        {
            anim.SetInteger("State", 2);

        }else if(rb.velocity.y <=-.1f)
        {
            anim.SetInteger("State", 3);
        }
    }

    bool CheckRangeForDestroyEnemy()
    {
        if(Enemy!=null)
        {
            if (Vector2.Distance(transform.position, Enemy.transform.position)<=2f)
            {
                return true;
            }

        }

        return false;
    }


   
    void Restart()
    {
      
        rb.bodyType = RigidbodyType2D.Static;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            Death = true;
            anim.SetBool("Death", true);
        }
    }
}
