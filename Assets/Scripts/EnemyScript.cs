using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyScript : AbstractEnemy
{
    private const int RAYSARRAYSIZE= 2;
    private const int HITINDEX = 0;

    private Animator anim;
    private int lifeCounter = 0;
    private bool isNotdead = true;
    private WayPointsMovement wayPointsMovementScript;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private EnemyObserverListener enemyObserverListener;
    private const int HITPOINTS = 10;
    private RaycastHit2D[] rayReleased;
    private ContactFilter2D contactFilter2D;
    private Collider2D playerCollider;

    [SerializeField] LayerMask Player;
    [Header("Max Health For The Enemy")]
    [SerializeField] int MaxHealth;
    [Header("Hittable Objects")]
    [SerializeField] public EnemyHittableObjects _enemyHittableObjects;
    [Header("Enter hit Param name")]
    [SerializeField] string animationHitParam;
    [Header("Enter Attack Anim Param name")]
    [SerializeField] string animationAttackParam;
    [SerializeField] string[] extraAnimations;

    public override string enemyName { get => m_Name; set => m_Name = value; }
    public override int health { get => m_health; set => m_health = value; }
    public override int maxHealth { get => m_maxHealth; set => m_maxHealth = value; }


    private void Awake()
    {
        rayReleased = new RaycastHit2D[RAYSARRAYSIZE];
        contactFilter2D.SetLayerMask(Player);
        enemyName = gameObject.name;
        maxHealth = MaxHealth;
        health = maxHealth;
        wayPointsMovementScript = gameObject.GetComponent<WayPointsMovement>();
        enemyObserverListener = FindObjectOfType<EnemyObserverListener>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

    }

    async void Update()
    {

        if (await isPlayerInSight())
        {
            wayPointsMovementScript.shouldMove = false;

            await enemyObserverListener.EnemyActionDelegator(playerCollider, gameObject, animationAttackParam, true);

        }
        else
        {
            wayPointsMovementScript.shouldMove = true;
        }

        if (isEnemyHealthZero(health))
        {
            if (!cancellationTokenSource.IsCancellationRequested)
                Destroy(gameObject);
        }
    }

    private async Task<bool> isPlayerInSight()
    {
        Debug.DrawRay(transform.position, transform.right * 3f, Color.cyan);

        await Task.Delay(System.TimeSpan.FromSeconds(.1f));

        if (!cancellationToken.IsCancellationRequested)
        {
            int numOfObjectsInContact=Physics2D.Raycast(transform.position,  transform.right, contactFilter2D, rayReleased, 3f);

            if(numOfObjectsInContact > 0) { playerCollider = rayReleased[HITINDEX].collider; return true; }

        }

        return false;
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject != null && await EnemyHittableManager.isEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            health -= HITPOINTS;
            _ = await enemyObserverListener.EnemyActionDelegator(collision, gameObject, animationHitParam, true);

        }
    }
    private async void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject != null && await EnemyHittableManager.isEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            _ = await enemyObserverListener.EnemyActionDelegator(collision, gameObject, animationHitParam, false);

        }
    }
    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

    private bool isEnemyHealthZero(int enemyHealth)
    {
       return enemyHealth==0? true: false;
    }

}
