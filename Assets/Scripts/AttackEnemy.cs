using UnityEngine;

public class AttackEnemy : MonoBehaviour
{

    private float DaggerSpeed = 20f;
    private Rigidbody2D rb;
    private Animator anim;
    private float elapsedTime = 0;
    private bool checker = true;
    private GameObject Heroine;


    private SpriteRenderer _daggerrenderer;
    public static bool ThrowDagger;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _daggerrenderer = GetComponent<SpriteRenderer>();
        Heroine = GameObject.FindWithTag("Player");
    }

    void Update()
    {


        if (checker)
        {
            elapsedTime += Time.deltaTime;

        }

        if (elapsedTime > 1f)
        {
            checker = false;
            anim.SetBool("HitEnemy", true);
            Destroy(gameObject, .4f);
            elapsedTime = 0;

        }

        if (Heroine.GetComponent<SpriteRenderer>().flipX && ThrowDagger)
        {
            _daggerrenderer.flipX = true;
            rb.velocity = new Vector2(-DaggerSpeed, 0);
            ThrowDagger = false;



        }

        if (!Heroine.GetComponent<SpriteRenderer>().flipX && ThrowDagger)
        {
            _daggerrenderer.flipX = false;
            rb.velocity = new Vector2(DaggerSpeed, 0);
            ThrowDagger = false;


        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Destroy(collision.gameObject);
            anim.SetBool("HitEnemy", true);
            Destroy(gameObject, .4f);


        }

    }
}
