using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using EnemyHittable;
using static GameObjectCreator;
using static SceneData;
public class EnemyScript : AbstractEntity
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

    public override string EntityName { get => m_Name; set => m_Name = value; }
    public override float Health { get => m_health; set => m_health = value; }
    public override float MaxHealth { get => m_maxHealth; set => m_maxHealth = value; }


    private void Awake()
    {
        rayReleased = new RaycastHit2D[RAYSARRAYSIZE];
        contactFilter2D.SetLayerMask(Player);
        EntityName = gameObject.name;
        MaxHealth = maxHealth;
        Health = maxHealth;
        wayPointsMovementScript = gameObject.GetComponent<WayPointsMovement>();
   
    }

    void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        if(this!=null)
         InsertIntoGameStateHandlerList(this);
    }

    async void Update()
    {

        if (await isPlayerInSight())
        {
            wayPointsMovementScript.shouldMove = false;

            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

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

        if (isEnemyHealthZero(Health))
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
        if (gameObject != null && await EnemyHittableManager.isEntityAnAttackObject(collision, _enemyHittableObjects))
        {
            Health -= HITPOINTS;
            _ = await GetEnemyOberverListenerObject().EnemyActionDelegator(collision, gameObject, animationHitParam, true);

        }
    }
    private async void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject != null && await EnemyHittableManager.isEntityAnAttackObject(collision, _enemyHittableObjects))
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
        Debug.Log(enemyData.ToString());
        data.AddToObjectsToPersist(enemyData);
    }
}
