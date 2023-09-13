using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyScript : AbstractEnemy
{
    private Animator anim;
    private int lifeCounter = 0;
    private bool isNotdead = true;
    private SpriteRenderer sr;
    private WayPointsMovement wayPointsMovementScript;
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
        wayPointsMovementScript= gameObject.GetComponent<WayPointsMovement>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        cancellationTokenSource = new CancellationTokenSource();    
        cancellationToken = cancellationTokenSource.Token;

    }

    async void Update()
    {
        if (transform.gameObject.name != "Enemy2")
        {
            if (await isPlayerInSight())
            {
                wayPointsMovementScript.shouldMove = false;

                if(!cancellationToken.IsCancellationRequested) {

                    if (sr.flipX)
                    {
                        anim.SetBool("EnemyAttack2", true);

                    }
                    else
                    {
                        anim.SetBool("EnemyAttack", true);

                    }
                }

            }
            else
            {
                if (!cancellationToken.IsCancellationRequested)
                {

                        if (sr.flipX)
                    {
                        anim.SetBool("EnemyAttack2", false);

                    }
                    else
                    {
                        anim.SetBool("EnemyAttack", false);

                    }

                    wayPointsMovementScript.shouldMove = true;

                }
            }


        }

        if (lifeCounter > 3)
        {
            isNotdead = false;
            Destroy(gameObject, .5f);
            lifeCounter = 0;
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
