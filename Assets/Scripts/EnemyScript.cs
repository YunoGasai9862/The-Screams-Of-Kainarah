using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyScript : AbstractEnemy
{
    private Animator anim;
    private int lifeCounter = 0;
    private bool isNotdead = true;
    private float Speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject Heroine;
    [SerializeField] bool StopForAttack = false;
    public static RaycastHit2D[] hit;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    [SerializeField] LayerMask Player;
    [SerializeField] GameObject Hit;
    [Header("Max Health For The Enemy")]
    [SerializeField] int MaxHealth;

    public override string enemyName { get => m_Name; set => m_Name=value; }
    public override int health { get => m_health; set => m_health = value; }
    public override int maxHealth { get => m_maxHealth; set => m_maxHealth = value; }

    private void Awake()
    {
        enemyName= gameObject.name;
        maxHealth = MaxHealth;
        m_health = maxHealth;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Heroine = GameObject.FindWithTag("Player");
        cancellationTokenSource = new CancellationTokenSource();    
        cancellationToken = cancellationTokenSource.Token;

    }

    async void Update()
    {
        if (transform.gameObject.name != "Enemy2")
        {
            if (await isPlayerInSight())
            {
                StopForAttack = true;
                rb.velocity = new Vector2(0, 0);

                if (sr.flipX)
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

        if (!StopForAttack) //continuing moving if the Player is not in the range
        {
            //transform.position = Vector3.MoveTowards(transform.position, Waypoints[Index].position, Speed * Time.deltaTime);

        }

    }


    private async Task<bool> isPlayerInSight()
    {
        int sign = sr.flipX ? -1 : 1;

        Debug.DrawRay(transform.position, sign * transform.right * 3f, Color.cyan);

        await Task.Delay(System.TimeSpan.FromSeconds(1f));

        if (!cancellationToken.IsCancellationRequested)
         return Physics2D.Raycast(transform.position, sign * transform.right, 3f, Player);

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger") || collision.CompareTag("Sword"))
        {
            if (isNotdead)
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

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

}
