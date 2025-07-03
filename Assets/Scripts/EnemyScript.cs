using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using EnemyHittable;
using static SceneSingleton;
using static SceneData;
public class EnemyScript : AbstractEntity, IObserver<EnemyHittableManager>
{
    private const int RAYSARRAYSIZE= 2;
    private const int HITINDEX = 0;

    private WayPointsMovement wayPointsMovementScript;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private const int HITPOINTS = 10;
    private RaycastHit2D[] rayReleased;
    private ContactFilter2D contactFilter2D;
    private Collider2D playerCollider;
    private Collider2D tempCollider;

    [SerializeField] LayerMask Player;
    [Header("Max Health For The Enemy")]
    [SerializeField] int maxHealth;
    [Header("Hittable Objects")]
    [SerializeField] public EnemyHittableObjects _enemyHittableObjects;
    [Header("Enter hit Param name")]
    [SerializeField] string animationHitParam;
    [Header("Enter Attack Anim Param name")]
    [SerializeField] string animationAttackParam;
    [SerializeField] string[] extraAnimations;
    [SerializeField] EnemyHittableManagerDelegator enemyHittableManagerDelegator;

    public override Health Health { get; set; }

    private EnemyHittableManager EnemyHittableManager { get; set; }

    private void Awake()
    {
        rayReleased = new RaycastHit2D[RAYSARRAYSIZE];
        contactFilter2D.SetLayerMask(Player);
        wayPointsMovementScript = gameObject.GetComponent<WayPointsMovement>();
    }

    void Start()
    {

        Health = new Health()
        {
            CurrentHealth = maxHealth,
            MaxHealth = maxHealth,
            EntityName = name
        };

        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        //InsertIntoGameStateHandlerList(this);

        StartCoroutine(enemyHittableManagerDelegator.NotifySubject(this, new NotificationContext()
        {
            SubjectType = typeof(EnemyHittableManager).ToString(),
            ObserverName = name,
            ObserverTag = tag

        }, CancellationToken.None));
    }

    async void Update()
    {

        if (await isPlayerInSight())
        {
            wayPointsMovementScript.shouldMove = false;

            gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

            if(!cancellationToken.IsCancellationRequested)
                await GetEnemyOberverListenerObject().EnemyActionDelegator(playerCollider, gameObject, animationAttackParam, true);

        }
        else
        {
            wayPointsMovementScript.shouldMove = true;

            if (tempCollider!=null)
                if(!cancellationToken.IsCancellationRequested)    
                    await GetEnemyOberverListenerObject().EnemyActionDelegator(tempCollider, gameObject, animationAttackParam, false);

        }

        if (isEnemyHealthZero(Health.CurrentHealth))
        {
            if (!cancellationToken.IsCancellationRequested)
                Destroy(gameObject);
        }
    }

    private async Task<bool> isPlayerInSight()
    {
        Debug.DrawRay(transform.position, transform.right * 3f, Color.cyan);

        await Task.Delay(System.TimeSpan.FromSeconds(.1f));

        if(!cancellationToken.IsCancellationRequested)
        {
            int numOfObjectsInContact = Physics2D.Raycast(transform.position, transform.right, contactFilter2D, rayReleased, 3f);

            if (numOfObjectsInContact > 0)
            {
                playerCollider = rayReleased[HITINDEX].collider;
                tempCollider = playerCollider;
                return true;

            }
        }

        return false;
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        //another better way to avoid the null checks
        //make sure delegates are done during the preloading time!!
        if (EnemyHittableManager == null)
        {
            Debug.Log("EnemyHittableManager is null for [EnemyScript - OnTriggerEnter2D] - exiting!");
            return;
        }

        if (gameObject != null && await EnemyHittableManager.IsEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            Health.CurrentHealth -= HITPOINTS;
            _ = await GetEnemyOberverListenerObject().EnemyActionDelegator(collision, gameObject, animationHitParam, true);

        }
    }
    private async void OnTriggerExit2D(Collider2D collision)
    {
        if (EnemyHittableManager == null)
        {
            Debug.Log("EnemyHittableManager is null for [EnemyScript - OnTriggerExit2D] - exiting!");
            return;
        }

        if (gameObject != null && await EnemyHittableManager.IsEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            _ = await GetEnemyOberverListenerObject().EnemyActionDelegator(collision, gameObject, animationHitParam, false);

        }
    }
    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

    private bool isEnemyHealthZero(float enemyHealth)
    {
       return enemyHealth==0? true: false;
    }

    public override void GameStateHandler(SceneData data)
    {
        ObjectData enemyData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation);

        data.AddToObjectsToPersist(enemyData);
    }

    public void OnNotify(EnemyHittableManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EnemyHittableManager = data;
    }
}
