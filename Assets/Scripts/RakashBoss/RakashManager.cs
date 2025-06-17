using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static SceneData;
public class RakashManager : AbstractEntity, IObserver<GameState>, IObserver<Player>, IGameStateHandler
{
    private const float MAX_HEALTH = 100f;
    private GameState CurrentGameState { get; set; }
    public override string EntityName { get => m_Name; set => m_Name = value; }
    public override float Health { get => m_health; set => m_health = value; }
    public override float MaxHealth { get => m_maxHealth; set => m_maxHealth = value; }
    private Player Player { get; set; }

    [SerializeField] GameObject bossDead;
    [SerializeField] GlobalGameStateDelegator gameStateDelegator;
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private void Awake()
    {
        MaxHealth = Health = MAX_HEALTH;
    }

    void Start()
    {
        StartCoroutine(gameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None));

        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()

        }, CancellationToken.None));

        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    private Task<GameObject> HandleBossDefeatScenario(Vector3 position, GameObject prefab, GameObject mainObject)
    {
        GameObject dead = Instantiate(prefab, position, Quaternion.identity);

        return Task.FromResult(dead);   
    }
    private Task DestroyMultipleGameObjects(GameObject[] gameObjects, float destroyInSeconds)
    {
       foreach(var gameObject in gameObjects)
       {
            Destroy(gameObject, destroyInSeconds);
       }
       return Task.CompletedTask;
    }

    public override void GameStateHandler(SceneData data)
    {
        ObjectData bossData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation);

        data.AddToObjectsToPersist(bossData);
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
