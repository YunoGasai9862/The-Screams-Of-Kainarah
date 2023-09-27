using System.Collections.Generic;
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
    private EnemyHittableManager enemyHittableManager;

    [SerializeField] LayerMask Player;
    [SerializeField] GameObject Hit;
    [Header("Max Health For The Enemy")]
    [SerializeField] int MaxHealth;
    [Header("Hittable Objects")]
    [SerializeField] public EnemyHittableObjects _enemyHittableObjects;

    private SubjectsToBeNotified<Collider2D> colliderEnemySubjects;

    public override string enemyName { get => m_Name; set => m_Name = value; }
    public override int health { get => m_health; set => m_health = value; }
    public override int maxHealth { get => m_maxHealth; set => m_maxHealth = value; }


    private void Awake()
    {
        enemyName = gameObject.name;
        maxHealth = MaxHealth;
        m_health = maxHealth;
        wayPointsMovementScript = gameObject.GetComponent<WayPointsMovement>();
        enemyHittableManager = GameObject.FindObjectOfType<EnemyHittableManager>(); //from the scene
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

                //add attack logic here

            }
            else
            {
                wayPointsMovementScript.shouldMove = true;
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

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (await EnemyHittableManager.isEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            //observer pattern for animation and instantiators

            colliderEnemySubjects.NotifyObservers(ref collision);

            // GameObject endofLife = Instantiate(Hit, transform.position, Quaternion.identity);

            //add hit animation logic here

            // Destroy(endofLife, 1f);

            Debug.Log(collision.tag);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger") || collision.CompareTag("Sword"))
        {
            //observer pattern for animation


            //animation logic (a different class)
            //anim.SetBool("Hit", false);

        }
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

}
